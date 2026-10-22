$ErrorActionPreference='Stop'
$out='C:\Users\feder\Documents\SIGAT\entregables\modelo_actualizado_20261021'
$c=New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=$out\SIGAT_Modelo_Actualizado.EAP;Mode=Read;")
$c.Open()
function Rows($sql){$cmd=$c.CreateCommand();$cmd.CommandText=$sql;$r=$cmd.ExecuteReader();$result=@();while($r.Read()){$v=@{};for($i=0;$i -lt $r.FieldCount;$i++){$v[$r.GetName($i)]=$r.GetValue($i)};$result+=$v};$r.Close();return $result}
try{
 $data=@{
  objects=@(Rows 'SELECT Object_ID,Package_ID,Name,Object_Type,Stereotype FROM t_object')
  packages=@(Rows 'SELECT Package_ID,Name FROM t_package')
  diagrams=@(Rows 'SELECT Diagram_ID,Name,Diagram_Type FROM t_diagram')
  members=@(Rows 'SELECT Object_ID,Name,Type FROM t_attribute')
  connectors=@(Rows 'SELECT Connector_ID,Start_Object_ID,End_Object_ID,Connector_Type FROM t_connector')
  placements=@(Rows 'SELECT Diagram_ID,Object_ID FROM t_diagramobjects')
 }
 $data|ConvertTo-Json -Depth 6|Set-Content (Join-Path $out 'fuentes\inspeccion_eap.json') -Encoding UTF8
 Write-Output ('Diagramas: '+$data.diagrams.Count+'; tablas: '+@($data.objects|Where-Object Stereotype -eq 'table').Count+'; conectores: '+$data.connectors.Count)
}finally{$c.Close()}
