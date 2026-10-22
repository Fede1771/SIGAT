$ErrorActionPreference='Stop'
$ProgressPreference='SilentlyContinue'
$work='C:\Users\feder\Documents\SIGAT'
$out=Join-Path $work 'entregables\modelo_actualizado_20261021'
$model=Get-Content (Join-Path $out 'fuentes\modelo.json') -Raw -Encoding UTF8 | ConvertFrom-Json
$project=Join-Path $out 'SIGAT_Modelo_Actualizado.EAP'
if(Test-Path -LiteralPath $project){throw 'El archivo de salida ya existe; conservarlo y elegir otro nombre antes de generar.'}
New-Item -ItemType Directory -Force -Path (Join-Path $out 'diagramas') | Out-Null
Copy-Item -LiteralPath 'C:\Program Files (x86)\Sparx Systems\EA\EABase.eap' -Destination $project
$script:repo=$null
$script:elements=@{};$script:relations=@();$script:tables=@{};$script:packages=@{};$script:exported=@()
function Check($ok,$context){if(!$ok){throw "Enterprise Architect: $context"}}
function Package($parent,$name,$notes=''){
 $p=$parent.Packages.AddNew($name,'Package');$p.Notes=$notes;Check ($p.Update()) $name;return $p
}
function Element($p,$name,$kind='Class',$notes=''){
 $e=$p.Elements.AddNew($name,$kind);$e.Notes=$notes;Check ($e.Update()) $name;return $e
}
function Rect($d,$e,$x,$y,$w,$h,$color=16777215){
 $obj=$d.DiagramObjects.AddNew("l=$x;r=$($x+$w);t=$y;b=$($y+$h);",'');$obj.ElementID=$e.ElementID
 $obj.Style="BCol=$color;BFol=0;LCol=10066329;LWth=1;font=Segoe UI;fontsz=90;"
 Check ($obj.Update()) 'posición';return $obj
}
function Note($p,$d,$text,$x,$y,$w=1000,$h=100){
 $n=Element $p '' 'Note' $text
 Rect $d $n $x $y $w $h 14811135 | Out-Null
}
function Link($d,$c){
 $l=$d.DiagramLinks.AddNew('','');$l.ConnectorID=$c.ConnectorID
 $l.Style='LWidth=1;';Check ($l.Update()) 'conector visible'
}
function Signature($m){return $m.name+'('+ (($m.parameters | ForEach-Object { $_.kind+' '+$_.name+' : '+$_.type }) -join ', ')+') : '+$m.type}
function ClassNotes($t){
 $f=($t.fields | Where-Object {!$_.generated} | ForEach-Object {$_.scope+' '+$_.name+' : '+$_.type}) -join "`n"
 $m=($t.methods | ForEach-Object {$_.scope+' '+(Signature $_)}) -join "`n"
 $classification=if($t.layer -eq 'SIGAT.BE'){'Entidades y contratos del dominio administrativo implementado.'}else{'Implementación y soporte técnico de la capa.'}
 return "$($t.full)`n$classification`nFuente: $($t.files -join ', ')`n`nSe muestran miembros principales; contrato completo extraído del código:`n$f`n`n$m"
}
try{
 $script:repo=New-Object -ComObject EA.Repository
 $repo.SuppressEADialogs=$true
 Check ($repo.OpenFile($project)) 'abrir el archivo nuevo'
 $repo.EnableUIUpdates=$false
 if($repo.Models.Count -eq 0){$root=$repo.Models.AddNew('SIGAT - Código vigente 2026-10-21','Package');$root.Update()|Out-Null}else{$root=$repo.Models.GetAt(0);$root.Name='SIGAT - Código vigente 2026-10-21';$root.Update()|Out-Null}
 $root.Notes='Modelo reconstruido desde 57 fuentes C# y el instalador SQL. No modifica la aplicación ni la base SIGAT. T06b, inventario de activos y módulos futuros se identifican como pendientes y no se modelan como implementados.';$root.Update()|Out-Null
 $classRoot=Package $root 'G06 Clases por capa' 'Separación entre entidades del dominio administrativo y aspectos técnicos. La lógica de activos del propósito general aún no está implementada.'
 foreach($layer in @('SIGAT.BE','SIGAT.BLL','SIGAT.DAL','SIGAT.SERVICIOS','SIGAT.UI')){$script:packages[$layer]=Package $classRoot $layer}
 foreach($t in $model.classes){
  $e=Element $packages[$t.layer] $t.name $t.kind (ClassNotes $t)
  if($t.abstr){$e.Abstract='1'}
  if($t.stat){$e.Stereotype='static'}
  if($t.name -in @('SesionServicio','IdiomaManager')){$e.Stereotype='Singleton'}
  $e.Gentype='C#';$e.Visibility=$t.scope;$e.Update()|Out-Null
  $elements[$t.name]=$e
 }
 foreach($t in $model.classes){
  $e=$elements[$t.name]
  foreach($f in $t.displayFields){
   $a=$e.Attributes.AddNew($f.name,$f.type);$a.Visibility=$f.scope;$a.IsStatic=[bool]$f.stat
   if($f.category -eq 'enum'){$a.Stereotype='enum'}
   if($elements.ContainsKey($f.type)){$a.ClassifierID=$elements[$f.type].ElementID}
   Check ($a.Update()) 'atributo'
  }
  foreach($m in $t.displayMethods){
   $op=$e.Methods.AddNew($m.name,$m.type);$op.Visibility=$m.scope;$op.IsStatic=[bool]$m.stat;$op.Abstract=[bool]$m.abstr;$op.Notes=Signature $m
   Check ($op.Update()) 'operación';$pos=0
   foreach($param in $m.parameters){$a=$op.Parameters.AddNew($param.name,$param.type);$a.Kind=$param.kind;$a.Position=$pos;if($param.optional){$a.Default=$param.optional};$a.Update()|Out-Null;$pos++}
  }
 }
 foreach($r in $model.edges){
  $a=$elements[$r.a];$b=$elements[$r.b]
  $nativeKind=if($r.kind -eq 'Realisation'){'Dependency'}else{$r.kind};$c=$a.Connectors.AddNew($r.label,$nativeKind);$c.SupplierID=$b.ElementID;$c.Direction='Source -> Destination'
  if($r.kind -eq 'Association'){$c.Name='';$c.SupplierEnd.Role=$r.label;$c.SupplierEnd.Cardinality=if($r.many){'0..*'}else{'0..1'}}
  Check ($c.Update()) 'relación de clase';if($r.kind -eq 'Realisation'){$repo.Execute("UPDATE t_connector SET Connector_Type='Realisation' WHERE Connector_ID=$($c.ConnectorID)")|Out-Null};$relations+=@{a=$r.a;b=$r.b;connector=$c;kind=$r.kind}
 }
 Write-Output ('Tipos nativos creados: '+$elements.Count)
 $dataRoot=Package $root 'G07 Modelo de datos' 'Modelo físico: cuentas y seguridad, idiomas y soporte técnico. No hay tablas de activos ni versiones T06b.'
 foreach($table in @($model.schema.columns.Tabla | Sort-Object -Unique)){
  $e=Element $dataRoot $table 'Class' ('Tabla dbo.'+$table+' del instalador. Las referencias FK se indican por columnas y multiplicidad.')
  $e.Stereotype='table';$e.Gentype='SQL Server';$e.Update()|Out-Null;$tables[$table]=$e
  foreach($col in @($model.schema.columns | Where-Object Tabla -eq $table)){
   $tipo=$col.Tipo
   if($tipo -in @('varchar','nvarchar','char','nchar','binary','varbinary')){$length=if($col.Longitud -eq -1){'max'}elseif($tipo -in @('nvarchar','nchar')){[string]([int]($col.Longitud/2))}else{[string]$col.Longitud};$tipo+='('+$length+')'}
   $a=$e.Attributes.AddNew($col.Columna,$tipo);$a.Visibility='Public';$a.LowerBound=if($col.Nulo){'0'}else{'1'};$a.UpperBound='1'
   if($col.PK){$a.Stereotype='PK'}
   if($col.Identidad){$a.Notes='IDENTITY; identificador conservado en recuperación.'}
   if($col.Columna -eq 'DigitoVerificador'){$a.Notes=if($table -in @('Usuarios','UsuarioRol')){'DVH SHA-256 activo; actualizado transaccionalmente.'}else{'Columna legada conservada por compatibilidad. Ya no se utiliza para DVH/DVV.'}}
   $a.Update()|Out-Null
  }
 }
 $tableRels=@()
 foreach($fk in $model.schema.foreignKeys){
  $c=$tables[$fk.Origen].Connectors.AddNew($fk.ColumnaOrigen+' -> '+$fk.ColumnaDestino,'Association');$c.SupplierID=$tables[$fk.Destino].ElementID;$c.Direction='Source -> Destination';$c.Stereotype='FK'
  $unique=@($model.schema.uniqueKeys | Where-Object {$_.Tabla -eq $fk.Origen -and $_.Columna -eq $fk.ColumnaOrigen -and $_.Indice -like 'UQ*'}).Count -gt 0
  if($fk.Origen -eq 'UsuarioIdioma'){$unique=$true}
  $c.ClientEnd.Cardinality=if($unique){'0..1'}else{'0..*'};$c.SupplierEnd.Cardinality='1';$c.Notes='Clave foránea '+$fk.Origen+'.'+$fk.ColumnaOrigen+' -> '+$fk.Destino+'.'+$fk.ColumnaDestino
  $c.Update()|Out-Null;$tableRels+=@{a=$fk.Origen;b=$fk.Destino;connector=$c}
 }
 $views=Package $root 'Módulos y recorridos implementados'
 $pending=Package $root 'Alcance y pendientes' 'T06b no se presenta como una implementación existente.'
 $colors=@{'SIGAT.BE'=15794160;'SIGAT.BLL'=16250343;'SIGAT.DAL'=16444129;'SIGAT.SERVICIOS'=15528443;'SIGAT.UI'=16770790}
 foreach($spec in $model.diagrams){
  $p=if($spec.group -eq 'G06 Clases por capa'){$packages[($spec.title -replace '^G06 Clases de ','')]}elseif($spec.kind -in @('er','runtime')){$dataRoot}elseif($spec.kind -eq 'notes'){$pending}else{$views}
  $native=if($spec.kind -eq 'sequence'){'Sequence'}elseif($spec.kind -eq 'usecase'){'Use Case'}elseif($spec.kind -eq 'components'){'Component'}else{'Logical'}
  $d=$p.Diagrams.AddNew($spec.title,$native);$d.Notes=$spec.notes;$d.ShowPublic=$true;$d.ShowPrivate=$true;$d.ShowProtected=$true;$d.ShowPackageContents=$false
  $d.ExtendedStyle='OpParams=0;ShowNotes=0;';$d.StyleEx='HideQuals=0;';$d.Update()|Out-Null
  $objects=@{};$maxbottom=0;$maxright=0
  if($spec.kind -eq 'class' -or $spec.kind -eq 'er'){
   $keys=@($spec.keys);$cols=if($keys.Count -le 5){2}elseif($keys.Count -le 12){3}else{4};$i=0
   foreach($key in $keys){
    $e=if($spec.kind -eq 'class'){$elements[$key]}else{$tables[$key]}
    $x=35+($i%$cols)*600;$y=50+[math]::Floor($i/$cols)*440
    $color=if($spec.kind -eq 'er'){16316664}else{($colors[($model.classes | Where-Object name -eq $key).layer])}
    $objects[$key]=Rect $d $e $x $y 525 370 $color
    $maxbottom=[math]::Max($maxbottom,$y+370);$maxright=[math]::Max($maxright,$x+525);$i++
   }
   $rlist=if($spec.kind -eq 'class'){$relations}else{$tableRels}
   foreach($r in $rlist){if($objects.ContainsKey($r.a) -and $objects.ContainsKey($r.b) -and !($spec.group -eq 'G06 Clases por capa' -and $r.kind -eq 'Dependency')){Link $d $r.connector}}
   Note $p $d $spec.notes 35 ($maxbottom+65) ([math]::Max(900,$maxright-35)) 115
  } elseif($spec.kind -eq 'sequence'){
   $x=65;$colwidth=365;$i=0
   foreach($node in $spec.nodes){
    $e=Element $p $node.title $node.kind ''
    if($node.classifier -and $elements.ContainsKey($node.classifier)){$e.ClassifierID=$elements[$node.classifier].ElementID;$e.Update()|Out-Null}
    $objects[$node.key]=$e;Rect $d $e $x 35 180 90 16777215 | Out-Null;$x+=$colwidth;$i++
   }
   $y=175;$num=0
   foreach($step in $spec.steps){
    if($step.note){$h=55+([math]::Ceiling($step.note.Length/110))*17;Note $p $d $step.note 50 $y ([math]::Max(850,$x-300)) $h;$y+=$h+45;continue}
    $a=$objects[$step.a];$b=$objects[$step.b];$num++
    $c=$a.Connectors.AddNew($step.text,'Sequence');$c.SupplierID=$b.ElementID;$c.Direction='Source -> Destination';$c.SequenceNo=$num;$c.Notes=$step.text;$c.Update()|Out-Null
    $messageKind=if($step.reply){'Reply'}else{'Synchronous'}
    $repo.Execute("UPDATE t_connector SET DiagramID=$($d.DiagramID),PtStartY=$y,PtEndY=$y,PDATA1='$messageKind' WHERE Connector_ID=$($c.ConnectorID)")|Out-Null
    Link $d $c
    $y+=90
   }
   Note $p $d $spec.notes 50 ($y+20) ([math]::Max(850,$x-300)) 95
  } elseif($spec.kind -in @('usecase','components')){
   $i=0
   foreach($node in $spec.nodes){
    $e=Element $p $node.title $node.kind '';$objects[$node.key]=$e
    if($spec.kind -eq 'usecase'){
     if($node.kind -eq 'Actor'){$x=50;$y=150;$w=180;$h=180}else{$x=390;$y=45+($i-1)*140;$w=510;$h=100}
    }else{$x=60+($i%2)*500;$y=55+[math]::Floor($i/2)*210;$w=350;$h=120}
    Rect $d $e $x $y $w $h 16250871 | Out-Null;$maxbottom=[math]::Max($maxbottom,$y+$h);$i++
   }
   foreach($r in $spec.edges){$c=$objects[$r.a].Connectors.AddNew($r.label,$r.kind);$c.SupplierID=$objects[$r.b].ElementID;$c.Direction=if($spec.kind -eq 'usecase'){'Unspecified'}else{'Source -> Destination'};$c.Update()|Out-Null;Link $d $c}
   Note $p $d $spec.notes 45 ($maxbottom+55) 950 145
  } else {
   Note $p $d $spec.notes 45 50 1100 350
  }
  $d.Update()|Out-Null;$repo.SaveDiagram($d.DiagramID)
  $path=Join-Path $out ('diagramas\'+$spec.id+'.png')
  $ok=$true # Las imágenes se exportan en un paso separado después de cerrar el modelo.
  Check $ok ('exportar '+$spec.id)
  $exported+=@{id=$spec.id;title=$spec.title;guid=$d.DiagramGUID;diagramId=$d.DiagramID;kind=$spec.kind;objects=$d.DiagramObjects.Count;links=$d.DiagramLinks.Count}
  Write-Output ($spec.id+' | '+$d.DiagramObjects.Count+' elementos | diagrama nativo guardado')
 }
 $repo.EnableUIUpdates=$true;$repo.RefreshModelView(0)
 $exported | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $out 'fuentes\exportaciones.json') -Encoding UTF8
 Write-Output ('Proyecto terminado: '+$project)
}finally{
 if($null -ne $script:repo){$repo.CloseFile();$repo.Exit();[void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($repo)}
}
