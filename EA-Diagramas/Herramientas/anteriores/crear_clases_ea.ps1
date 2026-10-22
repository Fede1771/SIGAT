$ErrorActionPreference = 'Stop'
$workspace = 'C:\Users\feder\Documents\SIGAT'
$salida = Join-Path $workspace 'entregables\clases_login_logout_EA'
New-Item -ItemType Directory -Path $salida -Force | Out-Null
$proyecto = Join-Path $salida 'SIGAT_Login_Logout.EAP'
$soloExportar=Test-Path -LiteralPath $proyecto
if (!$soloExportar) { Copy-Item -LiteralPath 'C:\Program Files (x86)\Sparx Systems\EA\EABase.eap' -Destination $proyecto }
$script:repo = $null
try {
    $script:repo = New-Object -ComObject EA.Repository
    $repo.SuppressEADialogs = $true
    if (!$repo.OpenFile($proyecto)) { throw 'Enterprise Architect no pudo abrir el proyecto nuevo.' }
    $repo.EnableUIUpdates = $false
    if($soloExportar) {
        $modelo=$repo.Models.GetAt(0)
        $firmas=@{
            'FrmLogin.BtnLogin_Click'=@('sender|object','e|EventArgs')
            'UsuarioBLL.Autenticar'=@('nombreUsuario|string','passwordPlana|string','usuarioValidado|Usuario|out')
            'UsuarioDAL.ObtenerPorNombreUsuario'=@('nombreUsuario|string')
            'RolesBLL.CargarRolesUsuario'=@('usuario|Usuario')
            'SesionServicio.IniciarSesion'=@('usuarioLogueado|Usuario')
            'BitacoraFactory.Crear'=@('identificadorUsuario|string','descripcion|string')
            'HashHelper.ObtenerHashSHA256'=@('texto|string')
            'Usuario.AsignarRol'=@('r|Rol')
            'FrmPrincipal.ItemLogout_Click'=@('sender|object','e|EventArgs')
            'BitacoraDAL.Insertar'=@('registro|Bitacora')
            'IntegridadDAL.Cambiar<T>'=@('tabla|string','cambiar|Func<SqlConnection, SqlTransaction, T>')
        }
        foreach($p in $modelo.Packages) {
            foreach($e in $p.Elements) {
                foreach($m in $e.Methods) {
                    $clave="$($e.Name).$($m.Name)"
                    $datos=$firmas[$clave]
                    if($clave -eq 'BitacoraBLL.Registrar') {
                        $datos=if($p.Name -like '*Logout'){@('usuario|string','actividad|string','informacion|string')}else{@('evento|IBitacoraEvento')}
                    }
                    if($null -eq $datos){continue}
                    while($m.Parameters.Count -gt 0) {$m.Parameters.DeleteAt(0,$false);$m.Parameters.Refresh()}
                    $pos=0
                    foreach($spec in $datos) {
                        $v=$spec.Split('|')
                        $a=$m.Parameters.AddNew($v[0],$v[1]);$a.Position=$pos
                        $a.Kind=if($v.Length -gt 2){$v[2]}else{'in'}
                        $a.Update() | Out-Null;$pos++
                    }
                    $m.Parameters.Refresh();$m.Update() | Out-Null
                }
                foreach($c in $e.Connectors) {
                    if($c.ClientID -eq $e.ElementID -and $c.Type -eq 'Association') {
                        $destino=$repo.GetElementByID($c.SupplierID)
                        $propiedades=@($e.Attributes | Where-Object {$_.Type -eq $destino.Name -and $_.Visibility -eq 'Private'})
                        if($propiedades.Count -eq 0){continue}
                        $c.Type='Aggregation';$c.Subtype='Strong'
                        $c.ClientID=$destino.ElementID;$c.SupplierID=$e.ElementID
                        $c.Direction='Unspecified';$c.Update() | Out-Null
                        $leido=$repo.GetConnectorByID($c.ConnectorID)
                        Write-Output "Composicion $($e.Name) -> $($destino.Name): $($leido.Type) $($leido.Subtype)"
                    }
                }
            }
            foreach($dg in $p.Diagrams) {
                $dg.ExtendedStyle=($dg.ExtendedStyle -replace 'OpParams=\d+;','')+'OpParams=2;'
                $dg.Update() | Out-Null
                foreach($o in $dg.DiagramObjects) {
                    $o.Style=($o.Style -replace 'fontsz=\d+;','')+'font=Arial;fontsz=130;'
                    $el=$repo.GetElementByID($o.ElementID)
                    if($p.Name -like '*Login' -and $el.Name -eq 'UsuarioBLL'){$o.Left=1120;$o.Right=2070}
                    if($p.Name -like '*Login' -and $el.Name -eq 'FrmLogin'){$o.Left=1345;$o.Right=1845}
                    $o.Update() | Out-Null
                }
                foreach($dl in $dg.DiagramLinks) {
                    $rel=$repo.GetConnectorByID($dl.ConnectorID)
                    if($rel.Type -eq 'Aggregation') {
                        $repo.Execute("UPDATE t_connector SET SourceIsAggregate=0, DestIsAggregate=2 WHERE Connector_ID=$($rel.ConnectorID)") | Out-Null
                    }
                }
                $repo.ReloadDiagram($dg.DiagramID)
                $nombre=if($dg.Name -like '*Logout'){'clases_logout'}else{'clases_login'}
                $ok=$repo.GetProjectInterface().PutDiagramImageToFile($dg.DiagramGUID,(Join-Path $salida "$nombre.png"),1)
                Write-Output "$($dg.Name): $($dg.DiagramObjects.Count) elementos; $($dg.DiagramLinks.Count) relaciones; exportacion=$ok"
            }
        }
        return
    }
    if ($repo.Models.Count -eq 0) {
        $modelo = $repo.Models.AddNew('SIGAT', 'Package'); $modelo.Update() | Out-Null
    } else { $modelo = $repo.Models.GetAt(0); $modelo.Name = 'SIGAT'; $modelo.Update() | Out-Null }
    $script:color = 220 + 231 * 256 + 247 * 65536
    $script:elementos = @{}
    function NuevaClase($nombre,$x,$y,$ancho,$alto,$tipo='Class',$estereotipo='') {
        $e=$script:paquete.Elements.AddNew($nombre,$tipo)
        $e.Stereotype=$estereotipo
        $e.Update() | Out-Null
        $o=$script:diagrama.DiagramObjects.AddNew("l=$x;r=$($x+$ancho);t=$y;b=$($y+$alto);",'')
        $o.ElementID=$e.ElementID
        $o.Style="BCol=$script:color;BFol=0;LCol=$([int](129 + 156*256 + 201*65536));LWth=1;"
        $o.Update() | Out-Null
        $script:elementos[$nombre]=$e
        return $e
    }
    function Atributo($clase,$nombre,$tipo,$visibilidad='Public') {
        $a=$script:elementos[$clase].Attributes.AddNew($nombre,$tipo)
        $a.Visibility=$visibilidad; $a.Update() | Out-Null
    }
    function Metodo($clase,$nombre,$retorno,$visibilidad='Public',$estatico=$false,$parametros=@()) {
        $m=$script:elementos[$clase].Methods.AddNew($nombre,$retorno)
        $m.Visibility=$visibilidad; $m.IsStatic=$estatico
        $m.Update() | Out-Null
        foreach($p in $parametros) {
            $arg=$m.Parameters.AddNew($p[0],$p[1])
            if($p.Count -gt 2){$arg.Kind=$p[2]}
            $arg.Update() | Out-Null
        }
        $m.Parameters.Refresh()
    }
    function Relacion($origen,$destino,$tipo='Dependency',$etiqueta='use') {
        $a=$script:elementos[$origen];$b=$script:elementos[$destino]
        $c=$a.Connectors.AddNew('',$tipo);$c.SupplierID=$b.ElementID
        $c.Direction='Source -> Destination'
        if($tipo -eq 'Aggregation'){$c.ClientEnd.Aggregation=2}
        elseif($tipo -eq 'Association'){$c.SupplierEnd.Role=$etiqueta;$c.SupplierEnd.Cardinality='0..1'}
        elseif($tipo -eq 'Dependency'){$c.Name=$etiqueta}
        $c.Update() | Out-Null
        $l=$script:diagrama.DiagramLinks.AddNew('','');$l.ConnectorID=$c.ConnectorID;$l.Update() | Out-Null
    }
    function PaqueteDiagrama($nombre) {
        $script:elementos=@{}
        $script:paquete=$modelo.Packages.AddNew($nombre,'Package');$paquete.Update() | Out-Null
        $script:diagrama=$paquete.Diagrams.AddNew($nombre,'Logical')
        $diagrama.ShowPublic=$true;$diagrama.ShowPrivate=$true;$diagrama.ShowProtected=$true
        $diagrama.ShowPackageContents=$false
        $diagrama.Update() | Out-Null
    }
    function Exportar($nombre) {
        $diagrama.DiagramObjects.Refresh();$diagrama.DiagramLinks.Refresh()
        $diagrama.Update() | Out-Null
        $repo.SaveDiagram($diagrama.DiagramID)
        $repo.GetProjectInterface().PutDiagramImageToFile($diagrama.DiagramGUID,(Join-Path $salida "$nombre.png"),1) | Out-Null
        Write-Output "$nombre : $($elementos.Count) elementos; $($diagrama.DiagramLinks.Count) relaciones"
    }
    PaqueteDiagrama 'T02 - Clases Login'
    NuevaClase 'FrmLogin' 1420 25 350 140 | Out-Null
    NuevaClase 'UsuarioBLL' 1250 245 690 150 | Out-Null
    NuevaClase 'ResultadoLogin' 25 520 270 180 'Enumeration' | Out-Null
    NuevaClase 'IntegridadBLL' 370 535 290 140 | Out-Null
    NuevaClase 'UsuarioDAL' 735 535 600 140 | Out-Null
    NuevaClase 'RolesBLL' 1410 535 480 140 | Out-Null
    NuevaClase 'BitacoraBLL' 1965 535 540 140 | Out-Null
    NuevaClase 'SesionServicio' 2580 510 440 180 'Class' 'Singleton' | Out-Null
    $f=NuevaClase 'BitacoraFactory' 3095 510 680 185
    $f.Abstract='1';$f.Update() | Out-Null
    NuevaClase 'HashHelper' 3850 535 530 140 'Class' 'static' | Out-Null
    NuevaClase 'Usuario' 1440 810 400 245 | Out-Null
    NuevaClase 'LoginExitosoFactory' 3070 855 270 100 | Out-Null
    NuevaClase 'LoginFallidoFactory' 3410 855 270 100 | Out-Null
    NuevaClase 'IBitacoraEvento' 3750 855 300 100 'Interface' | Out-Null
    Atributo 'FrmLogin' '_usuarioBLL' 'UsuarioBLL' 'Private'
    Metodo 'FrmLogin' 'BtnLogin_Click' 'void' 'Private' $false @(@('sender','object'),@('e','EventArgs'))
    Atributo 'UsuarioBLL' '_dal' 'UsuarioDAL' 'Private'
    Atributo 'UsuarioBLL' '_bitacora' 'BitacoraBLL' 'Private'
    Metodo 'UsuarioBLL' 'Autenticar' 'ResultadoLogin' 'Public' $false @(@('nombreUsuario','string'),@('passwordPlana','string'),@('usuarioValidado','Usuario','out'))
    foreach($valor in @('Exito','CredencialesInvalidas','UsuarioInactivo')) { Atributo 'ResultadoLogin' $valor '' }
    Metodo 'IntegridadBLL' 'ExigirIntegridad' 'void'
    Metodo 'UsuarioDAL' 'ObtenerPorNombreUsuario' 'Usuario' 'Public' $false @(@('nombreUsuario','string'))
    Metodo 'RolesBLL' 'CargarRolesUsuario' 'void' 'Public' $false @(@('usuario','Usuario'))
    Metodo 'BitacoraBLL' 'Registrar' 'void' 'Public' $false @(@('evento','IBitacoraEvento'))
    Atributo 'SesionServicio' 'UsuarioActual' 'Usuario'
    Metodo 'SesionServicio' 'ObtenerInstancia' 'SesionServicio' 'Public' $true
    Metodo 'SesionServicio' 'IniciarSesion' 'void' 'Public' $false @(@('usuarioLogueado','Usuario'))
    Metodo 'BitacoraFactory' 'Crear' 'IBitacoraEvento' 'Public' $false @(@('identificadorUsuario','string'),@('descripcion','string'))
    Metodo 'BitacoraFactory' 'CrearEvento' 'IBitacoraEvento' 'Protected'
    Metodo 'HashHelper' 'ObtenerHashSHA256' 'string' 'Public' $true @(@('texto','string'))
    foreach($a in @(@('IdUsuario','int'),@('NombreUsuario','string'),@('Password','string'),@('Activo','bool'))) { Atributo 'Usuario' $a[0] $a[1] }
    Metodo 'Usuario' 'AsignarRol' 'void' 'Public' $false @(@('r','Rol'))
    Relacion 'FrmLogin' 'UsuarioBLL' 'Aggregation' ''
    Relacion 'UsuarioBLL' 'UsuarioDAL' 'Aggregation' ''
    Relacion 'UsuarioBLL' 'BitacoraBLL' 'Aggregation' ''
    foreach($nombre in @('ResultadoLogin','IntegridadBLL','RolesBLL','SesionServicio','BitacoraFactory','HashHelper')) { Relacion 'UsuarioBLL' $nombre }
    Relacion 'UsuarioDAL' 'Usuario' 'Dependency' 'crea'
    Relacion 'SesionServicio' 'Usuario' 'Association' 'UsuarioActual'
    Relacion 'LoginExitosoFactory' 'BitacoraFactory' 'Generalization' ''
    Relacion 'LoginFallidoFactory' 'BitacoraFactory' 'Generalization' ''
    Relacion 'BitacoraFactory' 'IBitacoraEvento' 'Dependency' 'use'
    Exportar 'clases_login'

    PaqueteDiagrama 'T02 - Clases Logout'
    NuevaClase 'FrmPrincipal' 1320 25 520 230 | Out-Null
    NuevaClase 'BitacoraBLL' 165 390 870 210 | Out-Null
    NuevaClase 'SesionServicio' 1290 350 540 260 'Class' 'Singleton' | Out-Null
    NuevaClase 'FrmLogin' 1950 405 180 135 | Out-Null
    NuevaClase 'BitacoraDAL' 80 790 660 190 | Out-Null
    NuevaClase 'Bitacora' 815 755 510 270 | Out-Null
    NuevaClase 'IdiomaManager' 1400 770 510 220 'Class' 'Singleton' | Out-Null
    NuevaClase 'Usuario' 1960 800 370 160 | Out-Null
    NuevaClase 'IntegridadDAL' 80 1140 900 190 | Out-Null
    Atributo 'FrmPrincipal' '_bitacora' 'BitacoraBLL' 'Private'
    Metodo 'FrmPrincipal' 'ItemLogout_Click' 'void' 'Private' $false @(@('sender','object'),@('e','EventArgs'))
    Metodo 'FrmPrincipal' 'CerrarFormularioActual' 'void' 'Private'
    Atributo 'BitacoraBLL' '_dal' 'BitacoraDAL' 'Private'
    Metodo 'BitacoraBLL' 'Registrar' 'void' 'Public' $false @(@('usuario','string'),@('actividad','string'),@('informacion','string'))
    Atributo 'BitacoraDAL' 'integridad' 'IntegridadDAL' 'Private'
    Metodo 'BitacoraDAL' 'Insertar' 'void' 'Public' $false @(@('registro','Bitacora'))
    Metodo 'IntegridadDAL' 'Cambiar<T>' 'T' 'Public' $false @(@('tabla','string'),@('cambiar','Func<SqlConnection, SqlTransaction, T>'))
    Atributo 'SesionServicio' 'UsuarioActual' 'Usuario'
    Metodo 'SesionServicio' 'ObtenerInstancia' 'SesionServicio' 'Public' $true
    Metodo 'SesionServicio' 'CerrarSesion' 'void'
    Metodo 'IdiomaManager' 'ObtenerInstancia' 'IdiomaManager' 'Public' $true
    Metodo 'IdiomaManager' 'ReiniciarSesion' 'void'
    Atributo 'Usuario' 'NombreUsuario' 'string'
    foreach($a in @(@('Fecha','DateTime'),@('Usuario','string'),@('Actividad','string'),@('InformacionAsociada','string'))) { Atributo 'Bitacora' $a[0] $a[1] }
    Relacion 'FrmPrincipal' 'BitacoraBLL' 'Aggregation' ''
    Relacion 'FrmPrincipal' 'SesionServicio'
    Relacion 'FrmPrincipal' 'FrmLogin' 'Dependency' 'crea'
    Relacion 'BitacoraBLL' 'BitacoraDAL' 'Aggregation' ''
    Relacion 'BitacoraBLL' 'Bitacora' 'Dependency' 'crea'
    Relacion 'BitacoraDAL' 'IntegridadDAL' 'Aggregation' ''
    Relacion 'SesionServicio' 'IdiomaManager'
    Relacion 'SesionServicio' 'Usuario' 'Association' 'UsuarioActual'
    Exportar 'clases_logout'
    $repo.EnableUIUpdates=$true
    $repo.RefreshModelView(0)
    Write-Output "Proyecto creado: $proyecto"
} finally {
    if($null -ne $script:repo) { $repo.CloseFile();$repo.Exit();[void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($repo) }
}
