$ErrorActionPreference='Stop'
Add-Type -Path 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll'
Add-Type -ReferencedAssemblies 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll' -TypeDefinition @"
using System;
public static class ExportadorSIGAT {
 public static EA.IRepository repo; static EA.IProject proyecto;
 public static void Abrir(string archivo){repo=(EA.IRepository)new EA.RepositoryClass();repo.SuppressEADialogs=true;repo.OpenFile(archivo);repo.EnableUIUpdates=true;proyecto=(EA.IProject)repo.GetProjectInterface();}
 public static int[] Exportar(int id,string ruta,bool distribuir){
  EA.IDiagram d=(EA.IDiagram)repo.GetDiagramByID(id);
  string guid=Convert.ToString(proyecto.GUIDtoXML(d.DiagramGUID));
  if(distribuir) proyecto.LayoutDiagramEx(guid,805437440,4,95,95,true);
  EA.ICollection objs=(EA.ICollection)d.DiagramObjects; EA.ICollection links=(EA.ICollection)d.DiagramLinks; objs.Refresh();links.Refresh();
  if(!proyecto.PutDiagramImageToFile(guid,ruta,1))throw new Exception("No se exportó "+ruta);
  return new int[]{objs.Count,links.Count};
 }
 public static void Cerrar(){if(repo!=null){repo.CloseFile();repo.Exit();repo=null;proyecto=null;}}
}
"@

$out='C:\Users\feder\Documents\SIGAT\entregables\modelo_actualizado_20261021'
$items=Get-Content (Join-Path $out 'fuentes\exportaciones.json') -Raw -Encoding UTF8 | ConvertFrom-Json
try{
 [ExportadorSIGAT]::Abrir((Join-Path $out 'SIGAT_Modelo_Actualizado.EAP'))
 $r=[ExportadorSIGAT]::repo
 $d=$r.GetDiagramByID(44);$p=$r.GetPackageByID($d.PackageID)
 $e=$p.Elements.AddNew('Bitacora_Historico','Class');$e.Stereotype='table';$e.Gentype='SQL Server';$e.Notes='Creada al archivar en IntegridadDAL.ArchivarBitacora y Querys/09_Limpieza_Bitacora.sql. No es un historial de versiones T06b. Sin FK ni IDENTITY; conserva los IDs originales.';$e.Update()|Out-Null
 foreach($c in @(@('IdBitacora','int','1'),@('Fecha','datetime','1'),@('Usuario','varchar(50)','1'),@('Actividad','varchar(255)','1'),@('InformacionAsociada','nvarchar(max)','0'),@('DigitoVerificador','nvarchar(250)','0'))){
  $a=$e.Attributes.AddNew($c[0],$c[1]);$a.LowerBound=$c[2];$a.UpperBound='1';$a.Visibility='Public';if($c[0] -eq 'IdBitacora'){$a.Stereotype='PK'};if($c[0] -eq 'DigitoVerificador'){$a.Notes='Legado; no participa del DVH/DVV.'};$a.Update()|Out-Null
 }
 $o=$d.DiagramObjects.AddNew('l=35;r=675;t=45;b=275;','');$o.ElementID=$e.ElementID;$o.Style='BCol=16577258;font=Segoe UI;fontsz=100;';$o.Update()|Out-Null
 $d.DiagramObjects.Refresh();foreach($o in $d.DiagramObjects){if($o.ElementID -ne $e.ElementID){$o.top=-320;$o.bottom=-465;$o.left=35;$o.right=1100;$o.Update()|Out-Null}}
 foreach($item in $items){
  $counts=[ExportadorSIGAT]::Exportar($item.diagramId,(Join-Path $out ('diagramas\'+$item.id+'.png')),$false)
  $item.objects=$counts[0];$item.links=$counts[1]
 }
 $items | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $out 'fuentes\exportaciones.json') -Encoding UTF8
}finally{[ExportadorSIGAT]::Cerrar()}
