using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

string root = Path.GetFullPath(args[0]), output = Path.GetFullPath(args[1]);
var types = new Dictionary<string, TypeInfo>();
var sources = new List<object>();
var parsed = new List<SyntaxTree>();
string Scope(SyntaxTokenList modifiers, string fallback = "Private") => modifiers.Any(SyntaxKind.PublicKeyword) ? "Public" : modifiers.Any(SyntaxKind.ProtectedKeyword) ? "Protected" : modifiers.Any(SyntaxKind.InternalKeyword) ? "Package" : fallback;
string Clean(SyntaxNode node) => node?.NormalizeWhitespace().ToFullString() ?? "";
foreach (string layer in new[] { "SIGAT.BE", "SIGAT.BLL", "SIGAT.DAL", "SIGAT.SERVICIOS", "SIGAT.UI" })
foreach (string path in Directory.EnumerateFiles(Path.Combine(root, layer), "*.cs", SearchOption.AllDirectories).Where(p => !p.Contains("\\obj\\") && !p.Contains("\\bin\\")))
{
    string source = File.ReadAllText(path);
    var tree = CSharpSyntaxTree.ParseText(source);
    parsed.Add(tree);
    if (tree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)) throw new Exception("Error de sintaxis en " + path);
    string relative = Path.GetRelativePath(root, path).Replace('\\', '/');
    sources.Add(new { path = relative, sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant() });
    foreach (var decl in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
    {
        string ns = string.Join(".", decl.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(n => n.Name.ToString()));
        string parent = string.Join(".", decl.Ancestors().OfType<BaseTypeDeclarationSyntax>().Reverse().Select(n => n.Identifier.Text));
        string name = (parent.Length > 0 ? parent + "." : "") + decl.Identifier.Text;
        string full = ns + "." + name;
        if (!types.TryGetValue(full, out var type)) types.Add(full, type = new TypeInfo { name = name, full = full, layer = layer, kind = decl is EnumDeclarationSyntax ? "Enumeration" : decl is InterfaceDeclarationSyntax ? "Interface" : "Class", abstr = decl.Modifiers.Any(SyntaxKind.AbstractKeyword), stat = decl.Modifiers.Any(SyntaxKind.StaticKeyword), scope = Scope(decl.Modifiers, "Package") });
        type.files.Add(relative);
        if (decl.BaseList != null) foreach (var b in decl.BaseList.Types) type.bases.Add(Clean(b.Type));
        if (decl is EnumDeclarationSyntax en) foreach (var value in en.Members) type.fields.Add(new Member { name = value.Identifier.Text, type = "", scope = "Public", category = "enum" });
        if (decl is not TypeDeclarationSyntax td) continue;
        if (td is RecordDeclarationSyntax rec && rec.ParameterList != null)
            foreach (var p in rec.ParameterList.Parameters) type.fields.Add(new Member { name = p.Identifier.Text, type = Clean(p.Type), scope = "Public", category = "property" });
        foreach (var m in td.Members)
        {
            if (m is PropertyDeclarationSyntax p) type.fields.Add(new Member { name = p.Identifier.Text, type = Clean(p.Type), scope = Scope(p.Modifiers, td is InterfaceDeclarationSyntax ? "Public" : "Private"), stat = p.Modifiers.Any(SyntaxKind.StaticKeyword), category = "property" });
            if (m is FieldDeclarationSyntax f) foreach (var v in f.Declaration.Variables) type.fields.Add(new Member { name = v.Identifier.Text, type = Clean(f.Declaration.Type), scope = Scope(f.Modifiers), stat = f.Modifiers.Any(SyntaxKind.StaticKeyword) || f.Modifiers.Any(SyntaxKind.ConstKeyword), category = "field", generated = relative.EndsWith(".Designer.cs") });
            if (m is MethodDeclarationSyntax method) type.methods.Add(new Operation { name = method.Identifier.Text + Clean(method.TypeParameterList), type = Clean(method.ReturnType), scope = Scope(method.Modifiers, td is InterfaceDeclarationSyntax ? "Public" : "Private"), stat = method.Modifiers.Any(SyntaxKind.StaticKeyword), abstr = method.Modifiers.Any(SyntaxKind.AbstractKeyword), parameters = method.ParameterList.Parameters.Select(p => new Parameter { name = p.Identifier.Text, type = Clean(p.Type), kind = p.Modifiers.Any(SyntaxKind.OutKeyword) ? "out" : p.Modifiers.Any(SyntaxKind.RefKeyword) ? "inout" : "in", optional = p.Default?.Value.ToString() ?? "" }).ToList() });
        }
        foreach (var id in td.Members.Where(m => m is not BaseTypeDeclarationSyntax).SelectMany(m => m.DescendantNodes().OfType<IdentifierNameSyntax>())) type.uses.Add(id.Identifier.Text);
    }
}
// Resolve project type symbols so a property called Usuario is not mistaken for a dependency on the Usuario class.
var common = CSharpSyntaxTree.ParseText("global using System; global using System.Collections.Generic; global using System.Linq; global using System.Threading; global using System.Threading.Tasks;");
var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p => MetadataReference.CreateFromFile(p));
var compilation = CSharpCompilation.Create("AnalisisSIGAT", parsed.Append(common), references);
foreach (var t in types.Values) t.uses.Clear();
foreach (var tree in parsed)
{
    var semantic = compilation.GetSemanticModel(tree, true);
    foreach (var decl in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
    {
        var owner = semantic.GetDeclaredSymbol(decl)?.ToDisplayString();
        if (owner == null || !types.TryGetValue(owner, out var type)) continue;
        foreach (var id in decl.Members.Where(m => m is not BaseTypeDeclarationSyntax).SelectMany(m => m.DescendantNodes().OfType<IdentifierNameSyntax>()))
            if (semantic.GetSymbolInfo(id).Symbol is INamedTypeSymbol target)
            {
                string fullTarget = target.OriginalDefinition.ToDisplayString();
                if (types.TryGetValue(fullTarget, out var referred) && referred.full != owner) type.uses.Add(referred.name);
            }
    }
}
foreach (var t in types.Values)
{
    t.files = t.files.Distinct().Order().ToList();
    t.fields = t.fields.GroupBy(f => f.name).Select(g => g.First()).ToList();
    t.methods = t.methods.GroupBy(m => m.name + "(" + string.Join(",", m.parameters.Select(p => p.type)) + ")").Select(g => g.First()).ToList();
    t.uses.Remove(t.name);
}
Directory.CreateDirectory(output);
File.WriteAllText(Path.Combine(output, "codigo_actual.json"), JsonSerializer.Serialize(new { generated = "2026-10-21", types = types.Values.OrderBy(t => t.full), sources }, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"{types.Count} tipos; {sources.Count} archivos; extracción sintáctica sin ejecutar la aplicación.");

class TypeInfo { public string name { get; set; } public string full { get; set; } public string layer { get; set; } public string kind { get; set; } public string scope { get; set; } public bool abstr { get; set; } public bool stat { get; set; } public List<string> files { get; set; } = new(); public HashSet<string> bases { get; set; } = new(); public HashSet<string> uses { get; set; } = new(); public List<Member> fields { get; set; } = new(); public List<Operation> methods { get; set; } = new(); }
class Member { public string name { get; set; } public string type { get; set; } public string scope { get; set; } public bool stat { get; set; } public bool generated { get; set; } public string category { get; set; } }
class Operation { public string name { get; set; } public string type { get; set; } public string scope { get; set; } public bool stat { get; set; } public bool abstr { get; set; } public List<Parameter> parameters { get; set; } = new(); }
class Parameter { public string name { get; set; } public string type { get; set; } public string kind { get; set; } public string optional { get; set; } }
