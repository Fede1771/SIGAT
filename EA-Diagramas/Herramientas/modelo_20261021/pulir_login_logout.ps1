$ErrorActionPreference='Stop'
Add-Type -Path 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll'
Add-Type -ReferencedAssemblies 'C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll' -TypeDefinition @"
using System;
using System.Collections.Generic;
public static class AjusteSIGAT {
 public static EA.IRepository R; static EA.IProject P;
 public static void Open(string path){R=(EA.IRepository)new EA.RepositoryClass();R.SuppressEADialogs=true;R.OpenFile(path);P=(EA.IProject)R.GetProjectInterface();}
 public static void Style(int id,bool layout){
  EA.IDiagram d=(EA.IDiagram)R.GetDiagramByID(id);d.ExtendedStyle="OpParams="+(layout?"0":"2")+";";d.Update();
  if(!layout)return;
  EA.ICollection links=(EA.ICollection)d.DiagramLinks;links.Refresh();
  for(short li=0;li<links.Count;li++){EA.IDiagramLink link=(EA.IDiagramLink)links.GetAt(li);EA.IConnector c=(EA.IConnector)R.GetConnectorByID(link.ConnectorID);if(Convert.ToString(c.Type)=="Dependency"){
   EA.IElement a=(EA.IElement)R.GetElementByID(c.ClientID),b=(EA.IElement)R.GetElementByID(c.SupplierID);
   bool keep=d.DiagramID==11 || d.DiagramID==12 || (Convert.ToString(a.Name)=="FormGestionRoles" && Convert.ToString(b.Name).StartsWith("Dlg")) || (Convert.ToString(a.Name)=="RolesBLL" && Convert.ToString(b.Name)=="MatrizRoles") || (Convert.ToString(a.Name)=="UsuarioBLL" && new List<string>{"HashHelper","RolesBLL","SesionServicio","BitacoraFactory","IntegridadBLL"}.Contains(Convert.ToString(b.Name))) || (Convert.ToString(a.Name)=="IdiomaBLL" && Convert.ToString(b.Name)=="IdiomaManager") || Convert.ToString(b.Name)=="VerificadorSHA256";
   link.IsHidden=!keep;link.Update();
  }}
  EA.ICollection objects=(EA.ICollection)d.DiagramObjects;objects.Refresh();var notes=new List<int>();
  for(short n=(short)(objects.Count-1);n>=0;n--){EA.IDiagramObject o=(EA.IDiagramObject)objects.GetAt(n);EA.IElement e=(EA.IElement)R.GetElementByID(o.ElementID);
   if(Convert.ToString(e.Type)=="Note"){notes.Add(e.ElementID);objects.DeleteAt(n,false);continue;}
   int rows=((EA.ICollection)e.Attributes).Count+((EA.ICollection)e.Methods).Count;int h=55+Math.Max(2,rows)*16;
   o.bottom=o.top-h;o.right=o.left+440;o.Style="BCol=16577258;BFol=0;LCol=10066329;LWth=1;font=Segoe UI;fontsz=100;";o.Update();
  }
  objects.Refresh();
  P.LayoutDiagramEx(Convert.ToString(P.GUIDtoXML(d.DiagramGUID)),805437440,4,70,75,true);
  objects.Refresh();int bottom=0,right=900;for(short oi=0;oi<objects.Count;oi++){EA.IDiagramObject o=(EA.IDiagramObject)objects.GetAt(oi);bottom=Math.Max(bottom,-o.bottom);right=Math.Max(right,o.right);}
  int y=bottom+45;foreach(int element in notes){EA.IDiagramObject o=(EA.IDiagramObject)objects.AddNew("l=35;r="+Math.Max(1000,right)+";t="+y+";b="+(y+105)+";","");o.ElementID=element;o.Update();y+=135;}
 }
 public static void Export(int id,string path){EA.IDiagram d=(EA.IDiagram)R.GetDiagramByID(id);if(!P.PutDiagramImageToFile(P.GUIDtoXML(d.DiagramGUID),path,1))throw new Exception("Export failed");}
 public static void Close(){if(R!=null){R.CloseFile();R.Exit();R=null;P=null;}}
}
"@

$out='C:\Users\feder\Documents\SIGAT\entregables\modelo_actualizado_20261021'
$model=Get-Content (Join-Path $out 'fuentes\modelo.json') -Raw -Encoding UTF8 | ConvertFrom-Json
try{
 [AjusteSIGAT]::Open((Join-Path $out 'SIGAT_Modelo_Actualizado.EAP'));$r=[AjusteSIGAT]::R
 [xml]$xml=$r.SQLQuery("SELECT o.Object_ID,o.Name FROM t_object o INNER JOIN t_package p ON o.Package_ID=p.Package_ID WHERE p.Name IN ('SIGAT.BE','SIGAT.BLL','SIGAT.DAL','SIGAT.SERVICIOS','SIGAT.UI') AND o.Object_Type<>'Note'")
 $elements=@{};foreach($row in $xml.EADATA.Dataset_0.Data.Row){$elements[[string]$row.Name]=$r.GetElementByID([int]$row.Object_ID)}
 $e=$elements['FrmPrincipal'];$op=$e.Methods.AddNew('CerrarFormularioActual','void');$op.Visibility='Private';$op.Update()|Out-Null
 foreach($id in @(11,12)){
  $d=$r.GetDiagramByID($id);$spec=$model.diagrams|Where-Object {$_.id -like ('{0:00}_*' -f $id)}
  $present=@{};$d.DiagramObjects.Refresh();foreach($o in $d.DiagramObjects){$present[[int]$o.ElementID]=$true}
  foreach($key in $spec.keys){$e=$elements[$key];if(!$present.ContainsKey([int]$e.ElementID)){$o=$d.DiagramObjects.AddNew('l=35;r=475;t=45;b=245;','');$o.ElementID=$e.ElementID;$o.Update()|Out-Null;$present[[int]$e.ElementID]=$true}}
  $linked=@{};$d.DiagramLinks.Refresh();foreach($l in $d.DiagramLinks){$linked[[int]$l.ConnectorID]=$true}
  [xml]$cx=$r.SQLQuery('SELECT Connector_ID,Start_Object_ID,End_Object_ID FROM t_connector')
  foreach($row in $cx.EADATA.Dataset_0.Data.Row){$cid=[int]$row.Connector_ID;if($present.ContainsKey([int]$row.Start_Object_ID) -and $present.ContainsKey([int]$row.End_Object_ID) -and !$linked.ContainsKey($cid)){$l=$d.DiagramLinks.AddNew('','');$l.ConnectorID=$cid;$l.Update()|Out-Null}}
  [AjusteSIGAT]::Style($id,$true);[AjusteSIGAT]::Export($id,(Join-Path $out ('diagramas\'+$spec.id+'.png')))
 }
}finally{[AjusteSIGAT]::Close()}
