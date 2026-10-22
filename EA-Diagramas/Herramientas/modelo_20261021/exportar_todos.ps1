$ErrorActionPreference='Stop'
Add-Type -Path 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll'
Add-Type -ReferencedAssemblies 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll' -TypeDefinition @"
using System;
public static class ExportadorSIGAT {
 static EA.IRepository repo; static EA.IProject proyecto;
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
 foreach($item in $items){
  $counts=[ExportadorSIGAT]::Exportar($item.diagramId,(Join-Path $out ('diagramas\'+$item.id+'.png')),$false)
  $item.objects=$counts[0];$item.links=$counts[1]
  Write-Output ($item.id+' | '+$item.objects+' elementos | exportado')
 }
 $items | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $out 'fuentes\exportaciones.json') -Encoding UTF8
}finally{[ExportadorSIGAT]::Cerrar()}
