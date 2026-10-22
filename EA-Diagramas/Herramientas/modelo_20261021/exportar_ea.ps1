$ErrorActionPreference='Stop'
$out='C:\Users\feder\Documents\SIGAT\entregables\modelo_actualizado_20261021'
$r=New-Object -ComObject EA.Repository
try{
$r.SuppressEADialogs=$true;$r.OpenFile((Join-Path $out 'SIGAT_Modelo_Actualizado.EAP'))|Out-Null
$r.EnableUIUpdates=$true
$items=Get-Content (Join-Path $out 'fuentes\exportaciones.json') -Raw -Encoding UTF8 | ConvertFrom-Json
foreach($item in $items){
$d=$r.GetDiagramByID($item.diagramId);$d.DiagramObjects.Refresh();$d.DiagramLinks.Refresh()
Write-Output ('Exportando '+$item.id+' con '+$d.DiagramObjects.Count+' elementos')
$ok=$r.GetProjectInterface().PutDiagramImageToFile(($r.GetProjectInterface().GUIDtoXML($d.DiagramGUID)),(Join-Path $out ('diagramas\'+$item.id+'.png')),1)
if(!$ok){throw ('Falló la imagen '+$item.id)}
$item.objects=$d.DiagramObjects.Count;$item.links=$d.DiagramLinks.Count
Write-Output ('Exportado '+$item.id)
}
$items | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $out 'fuentes\exportaciones.json') -Encoding UTF8
}finally{$r.CloseFile();$r.Exit();[void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($r)}
