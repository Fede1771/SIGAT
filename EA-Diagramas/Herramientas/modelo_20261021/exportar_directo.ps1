$ErrorActionPreference='Stop'
Add-Type -Path 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll'
Add-Type -ReferencedAssemblies 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll' -TypeDefinition @"
using System;
public static class ExportadorDirectoEA {
 public static void Exportar(string archivo,string carpeta) {
  EA.IRepository repo=(EA.IRepository)new EA.RepositoryClass();
  try {
   repo.SuppressEADialogs=true;
   repo.OpenFile(archivo);
   Console.WriteLine("Abierto con interfaz tipada");
   EA.IProject proyecto=(EA.IProject)repo.GetProjectInterface();
   Console.WriteLine("Interfaz de proyecto disponible");
   EA.IDiagram d=(EA.IDiagram)repo.GetDiagramByID(1);
   string guid=Convert.ToString(proyecto.GUIDtoXML(d.DiagramGUID));
   Console.WriteLine("GUID XML: "+guid);
   bool ok=proyecto.PutDiagramImageToFile(guid,carpeta+"\\01_arquitectura.png",1);
   Console.WriteLine("Imagen exportada: "+ok);
  } finally {repo.CloseFile();repo.Exit();}
 }
}
"@
[ExportadorDirectoEA]::Exportar('C:\Users\feder\Documents\SIGAT\entregables\modelo_actualizado_20261021\SIGAT_Modelo_Actualizado.EAP','C:\Users\feder\Documents\SIGAT\entregables\modelo_actualizado_20261021\diagramas')
