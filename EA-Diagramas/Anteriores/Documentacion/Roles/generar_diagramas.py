from pathlib import Path
from html import escape
import runpy
import re
import zipfile
from PIL import Image, ImageDraw, ImageFont

BASE = Path(__file__).resolve().parent
FONT = 'C:/Windows/Fonts/segoeui.ttf'
BOLD = 'C:/Windows/Fonts/segoeuib.ttf'

header = '''@startuml
skinparam backgroundColor white
skinparam shadowing false
skinparam defaultFontName Segoe UI
skinparam sequence {
ParticipantBackgroundColor #DAE8FC
ParticipantBorderColor #8AB2E2
ArrowColor #666666
GroupBorderColor #E6AE19
GroupHeaderBackgroundColor #FFF4CF
}
hide footbox
autonumber
'''
creation = header + '''title CU01 — Crear rol: lógica existente, entrada UI pendiente
actor Administrador as A
participant "form:FormGestionRoles" as F
participant "estado:EstadoRoles" as E
participant "nuevo:Rol" as R
participant "rolesBLL:RolesBLL" as B
participant "rolesDAL:RolesDAL" as D
participant "sesion:SesionServicio" as S
participant "MatrizRoles" as M
note over A,F: PENDIENTE: no hay botón actual que llame CrearRol.\nEntrada prevista: Ejecutar(() => CrearRol(false)).
A -> F: Solicitar creación de rol «entrada pendiente»
F -> F: Ejecutar(accion)
F -> F: CrearRol(false)
F -> F: Mostrar diálogo y validar nombre
break [cancelación o nombre inválido]
ref over A,F: Cancelación: sin guardar. Nombre inválido: mensaje y CargarDatos.\nVer Especificacion_roles.txt, alternativas A1 y A2.
end
F -> E: Roles {get}; calcular Max(Id) + 1
E --> F: List<Rol>
F -> R: new Rol { Id = ..., Nombre = texto }
F -> E: Roles.Add(nuevo) «operación de List<Rol>»
opt [variante CrearRol(true)]
ref over F,R: Destino() exige rol distinto de Administrador.\npadre.AgregarRol(nuevo) incorpora la relación anidada.
end
F -> B: Guardar(estado)
B -> D: Cargar()
D --> B: estado persistido actual
B -> B: ExigirGestion(estado persistido)
B -> S: ObtenerInstancia() «static»
S --> B: instancia
B -> S: UsuarioActual {get}
S --> B: usuario de sesión o null
ref over B,S: Usuario vigente y activo: TienePermiso(Administracion).\nVer detalle 03_Autorizacion del paquete previo.
break [gestor no autorizado]
ref over F,B: Excepción -> Ejecutar informa y CargarDatos recarga SQL.
end
B -> B: Validar(estado propuesto) «static»
B -> M: ValidarCatalogo(estado)
M --> B: void, si catálogo válido
note over B,M: Validar también conserva administrador original activo con rol 1,\nun administrador disponible y asignaciones dentro del catálogo.
break [validación falla]
ref over F,B: Excepción -> Ejecutar informa y recarga; no llama a Guardar de DAL.
end
B -> D: Guardar(estado)
note over E,D: Transacción Serializable; compara Version, protege al administrador,\nactualiza Rol y reconstruye relaciones. Commit y estado.Version++.
break [conflicto de versión o error de SQL]
ref over F,D: Sin Commit exitoso: no se confirma la operación.\nEjecutar informa y recarga el estado desde SQL.
end
D --> B: void, después de Commit
B --> F: void
F -> F: RefrescarArboles()
ref over F,R: Representa Roles y Permisos.\nVer CU02 y 04_Recorrido_Composite.
F --> A: Cambios guardados; nuevo rol vacío
note over A,M: Configurar el rol es otro caso de uso: cada acción tiene su guardado.\nNo se hacen participar hojas o familias en la creación de un rol vacío.
@enduml
'''
configure = header + '''title CU02 — Configurar rol: flujo accesible en la pantalla actual
actor Administrador as A
participant "form:FormGestionRoles" as F
participant "destino:Rol" as R
participant "familia:PermisoCompuesto" as C
participant "hoja:PermisoSimple" as H
participant "rolesBLL:RolesBLL" as B
participant "rolesDAL:RolesDAL" as D
A -> F: Elegir origen, rol destino y Asignar componente
F -> F: Ejecutar(AsignarARol)
F -> F: AsignarARol(); Destino()
break [destino no seleccionado o Id == 1]
ref over A,F: Excepción; mensaje y recarga. No se guarda.
end
alt [origen es Permiso: hoja o familia]
F -> R: AgregarPermiso(permiso)
note over R,H: Guarda la referencia mediante el contrato Permiso.\nNo invoca Agregar sobre la hoja ni sobre la familia.
else [origen es Rol]
F -> R: AgregarRol(rol)
ref over R,R: rol.ContieneRol(destino) rechaza ciclos; evita duplicados.
else [sin origen válido]
break [termina AsignarARol con excepción]
ref over A,F: Excepción; mensaje y recarga. No se guarda.
end
end
F -> B: Guardar(estado)
ref over B,D: Cargar -> ExigirGestion -> Validar -> Guardar.\nVer CU01: mismo camino de persistencia y control de errores.
B --> F: void, si guardado correcto
F -> F: RefrescarArboles()
loop [por cada rol del catálogo]
F -> F: CargarRol(nodos, rol)
F -> R: Permisos {get}
R --> F: List<Permiso>
loop [por cada permiso del rol]
F -> F: CargarNodosRecursivo(nodoPadre, permiso)
alt [objeto PermisoCompuesto]
F -> C: ObtenerHijos()
C --> F: List<Permiso>
loop [por cada hijo]
F -> F: CargarNodosRecursivo(nodo, hijo)
end
else [objeto PermisoSimple]
F -> H: ObtenerHijos()
H --> F: lista vacía; termina esta rama
end
end
F -> R: Roles {get}
R --> F: List<Rol>
ref over F,R: Por cada rol hijo: CargarRol(nodo.Nodes, hijo).\nTambién reconstruye catálogo y árbol del usuario.
end
F --> A: Composición actualizada y guardada
note over A,D: Editar familia es una acción separada.\nModificar una familia compartida afecta a todos sus roles.
A -> F: Seleccionar familia y Editar familia de permisos
F -> F: Ejecutar(EditarFamilia)
F -> C: ObtenerHijos()
C --> F: hijos directos actuales
F -> F: Mostrar lista y confirmar selección
loop [por cada hijo anterior]
F -> C: Quitar(hijo)
end
loop [por cada permiso marcado]
F -> C: Agregar(permiso)
ref over C,H: ContieneReferencia verifica ciclos recorriendo ObtenerHijos.\nLa hoja devuelve lista vacía; una familia devuelve descendientes.
end
F -> B: Guardar(estado)
ref over B,D: Mismas validaciones y transacción; ante error recarga SQL.
B --> F: void, si guardado correcto
F -> F: RefrescarArboles()
F --> A: Familia actualizada y guardada
@enduml
'''
for name, content in [('03_Crear_rol', creation), ('04_Configurar_rol', configure)]:
    lines = []
    for line in content.splitlines():
        if lines and lines[-1].startswith(('note over ', 'ref over ')) and line and not re.match(r'(?:\w+ -+>|note |ref |break |opt |alt |else |loop |end$|@)', line):
            lines[-1] += r'\n' + line
        else:
            lines.append(line)
    (BASE / (name + '.puml')).write_text('\n'.join(lines) + '\n', encoding='utf-8')

# Reutilizamos el estilo visual de las secuencias anteriores sin regenerarlas.
source = (BASE.parent / 'Diagramas' / 'renderizar.py').read_text(encoding='utf-8')
source = source.split("for path in sorted(BASE.glob('*.puml'))")[0]
scope = {'__file__': str(BASE / 'generar_diagramas.py')}
exec(compile(source, 'renderizador_secuencias', 'exec'), scope)
for name in ('03_Crear_rol', '04_Configurar_rol'):
    scope['render'](BASE / (name + '.puml'))

class Drawing:
    def __init__(self, width, height, title):
        self.width, self.height = width, height
        self.img = Image.new('RGB', (width, height), 'white')
        self.d = ImageDraw.Draw(self.img)
        self.svg = [f'<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}">', '<rect width="100%" height="100%" fill="white"/>']
        self.text(width/2, 32, title, 30, True, center=True)
    def text(self, x, y, text, size=21, bold=False, center=False):
        font = ImageFont.truetype(BOLD if bold else FONT, size)
        for i, part in enumerate(text.split('\n')):
            yy = y + i * (size + 9)
            xx = x - self.d.textlength(part, font=font)/2 if center else x
            self.d.text((xx, yy), part, font=font, fill='#20262e')
            self.svg.append(f'<text x="{xx}" y="{yy+size}" font-family="Segoe UI" font-size="{size}" font-weight="{700 if bold else 400}" fill="#20262e">{escape(part)}</text>')
    def rect(self, x, y, w, h, fill='#dae8fc', border='#8ab2e2'):
        self.d.rectangle((x,y,x+w,y+h), fill=fill, outline=border, width=2)
        self.svg.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{fill}" stroke="{border}" stroke-width="2"/>')
    def line(self, x1,y1,x2,y2,label='', dashed=False, inherit=False):
        self.d.line((x1,y1,x2,y2), fill='#666666', width=2)
        self.svg.append(f'<line x1="{x1}" y1="{y1}" x2="{x2}" y2="{y2}" stroke="#666666" stroke-width="2"'+(' stroke-dasharray="7 5"' if dashed else '')+'/>')
        if inherit:
            points=[(x2,y2),(x2-10,y2+20),(x2+10,y2+20)]
            self.d.polygon(points,fill='white',outline='#666666')
            self.svg.append(f'<polygon points="{x2},{y2} {x2-10},{y2+20} {x2+10},{y2+20}" fill="white" stroke="#666666"/>')
        if label: self.text((x1+x2)/2,(y1+y2)/2-26,label,16,center=True)
    def box(self, x,y,w,title,body,h=165):
        self.rect(x,y,w,h,'white')
        self.rect(x,y,w,43)
        self.text(x+w/2,y+6,title,22,True,True)
        self.text(x+14,y+55,body,19)
    def save(self,name):
        self.img.save(BASE/(name+'.png'))
        (BASE/(name+'.svg')).write_text('\n'.join(self.svg+['</svg>']),encoding='utf-8')

g=Drawing(1800,1280,'Arquitectura de roles y permisos — clases y relaciones reales')
g.box(30,110,470,'FormGestionRoles','Ejecutar(Action)\nCrearRol(bool) [sin entrada UI]\nAsignarARol() / EditarFamilia()',170)
g.box(650,110,450,'RolesBLL','CargarParaGestion() / Guardar(estado)\nExigirGestion(estado) [privado]\nValidar(estado) [static]',170)
g.box(1270,110,490,'RolesDAL','Cargar(): EstadoRoles\nGuardar(estado): void\nSQL: transacciones y Version',170)
g.line(500,195,650,195,'usa')
g.line(1100,195,1270,195,'usa')
g.box(30,375,470,'EstadoRoles','Version / IdAdministradorOriginal\nRoles: List<Rol>\nPermisos / Usuarios: listas',170)
g.box(650,375,450,'SesionServicio','ObtenerInstancia() [static]\nUsuarioActual: Usuario\nIdentidad del gestor',170)
g.box(1270,375,490,'MatrizRoles [static]','CrearCatalogo(): EstadoRoles\nValidarCatalogo(estado)\nReglas y Administrador protegido',170)
g.line(260,280,260,375,'mantiene')
g.line(830,280,830,375,'consulta')
g.line(1100,245,1270,395,'valida con')
g.box(30,685,470,'Usuario','Roles: List<Rol>\nAsignarRol() / QuitarRol()\nTienePermiso(nombre)',170)
g.box(650,685,450,'Rol','Roles: List<Rol>; Permisos: List<Permiso>\nAgregarRol() / AgregarPermiso()\nTienePermiso() / ContieneRol()',170)
g.box(1270,685,490,'Permiso [abstract]','ContienePermiso(nombre): bool\nAgregar() / Quitar()\nObtenerHijos(): List<Permiso>',170)
g.line(500,770,650,770,'0..* roles')
g.line(1100,770,1270,770,'0..* permisos')
g.line(260,545,260,685,'0..* usuarios')
g.line(750,685,750,615)
g.line(750,615,1010,615,'Rol -> Rol: 0..*')
g.line(1010,615,1010,685)
g.box(1070,1020,330,'PermisoSimple','Hoja: sin hijos\nObtenerHijos(): lista vacía\nAgregar/Quitar: excepción',165)
g.box(1440,1020,330,'PermisoCompuesto','hijos: List<Permiso>\nAgregar(): controla ciclos\nConsulta recursiva',165)
g.line(1235,1020,1235,950)
g.line(1235,950,1440,950)
g.line(1605,1020,1605,950)
g.line(1605,950,1440,950)
g.line(1440,950,1440,855,inherit=True)
g.text(30,1020,'Composite: Permiso es el componente común;\nPermisoSimple es la hoja y PermisoCompuesto la familia.\nRol contiene permisos; NO hereda de Permiso.\nFamilia -> Permiso: hijos compartidos, 0..*.\nSe muestran miembros relevantes, no la clase completa.',23)
g.save('02_Arquitectura_clases')

u=Drawing(1550,1080,'Casos de uso — gestión de roles y permisos')
u.rect(390,110,1110,870,'white','#666666')
u.text(945,130,'SIGAT',26,True,True)
u.d.ellipse((135,430,175,470),outline='#666666',width=3)
u.svg.append('<circle cx="155" cy="450" r="20" fill="white" stroke="#666666" stroke-width="3"/>')
for p in [(155,470,155,550),(115,505,195,505),(155,550,120,590),(155,550,190,590)]:u.line(*p)
u.text(155,610,'Gestor autorizado\nAdministración del sistema',21,False,True)
cases=[('CU01 Crear rol', True),('CU02 Configurar rol',False),('CU03 Editar familia de permisos',False),('CU04 Asignar / quitar rol a usuario',False),('CU05 Eliminar rol',True)]
for i,(label,pending) in enumerate(cases):
    y=235+i*145
    u.d.ellipse((650,y,1360,y+90),fill='#fff4cf' if pending else '#dae8fc',outline='#e6ae19' if pending else '#8ab2e2',width=2)
    u.svg.append(f'<ellipse cx="1005" cy="{y+45}" rx="355" ry="45" fill="'+('#fff4cf' if pending else '#dae8fc')+'" stroke="'+('#e6ae19' if pending else '#8ab2e2')+'" stroke-width="2"/>')
    u.text(1005,y+12,label,23,True,True)
    if pending:u.text(1005,y+47,'Lógica existente; acceso UI pendiente',19,False,True)
    u.line(195,510,650,y+45)
u.text(775,1000,'Azul: disponible. Amarillo: pendiente de entrada en interfaz.\nCrear y configurar son casos separados; no hay include obligatorio.',22,False,True)
u.save('01_Casos_de_uso')

(BASE/'01_Casos_de_uso.puml').write_text('''@startuml
skinparam backgroundColor white
skinparam shadowing false
skinparam usecaseBackgroundColor #DAE8FC
skinparam usecaseBorderColor #8AB2E2
left to right direction
actor "Gestor autorizado\nAdministración del sistema" as Gestor
rectangle SIGAT {
usecase "CU01 Crear rol\n[entrada UI pendiente]" as Crear #FFF4CF
usecase "CU02 Configurar rol" as Configurar
usecase "CU03 Editar familia de permisos" as Familia
usecase "CU04 Asignar / quitar rol a usuario" as Asignar
usecase "CU05 Eliminar rol\n[entrada UI pendiente]" as Eliminar #FFF4CF
}
Gestor -- Crear
Gestor -- Configurar
Gestor -- Familia
Gestor -- Asignar
Gestor -- Eliminar
note bottom of Crear : Crear no obliga a configurar inmediatamente.
@enduml
''',encoding='utf-8')
(BASE/'02_Arquitectura_clases.puml').write_text('''@startuml
skinparam backgroundColor white
skinparam shadowing false
skinparam classBackgroundColor #DAE8FC
skinparam classBorderColor #8AB2E2
class FormGestionRoles {
- Ejecutar(accion: Action)
- CrearRol(anidado: bool)
- AsignarARol()
- EditarFamilia()
}
class RolesBLL {
+ CargarParaGestion(): EstadoRoles
+ Guardar(estado: EstadoRoles)
- {static} ExigirGestion(estado: EstadoRoles)
+ {static} Validar(estado: EstadoRoles)
}
class RolesDAL {
+ Cargar(): EstadoRoles
+ Guardar(estado: EstadoRoles)
}
class EstadoRoles {
+ Version: long
+ IdAdministradorOriginal: int
+ Roles: List<Rol>
+ Permisos: List<Permiso>
+ Usuarios: List<Usuario>
}
class SesionServicio {
+ {static} ObtenerInstancia(): SesionServicio
+ UsuarioActual: Usuario
}
class MatrizRoles <<static>> {
+ {static} CrearCatalogo(): EstadoRoles
+ {static} ValidarCatalogo(estado: EstadoRoles)
}
class Usuario {
+ Roles: List<Rol>
+ AsignarRol(rol: Rol)
+ QuitarRol(rol: Rol)
+ TienePermiso(nombre: string): bool
}
class Rol {
+ Roles: List<Rol>
+ Permisos: List<Permiso>
+ AgregarRol(rol: Rol)
+ QuitarRol(rol: Rol)
+ AgregarPermiso(permiso: Permiso)
+ QuitarPermiso(permiso: Permiso)
+ ContieneRol(rol: Rol): bool
+ TienePermiso(nombre: string): bool
}
abstract class Permiso {
+ Id: int
+ Nombre: string
+ {abstract} ContienePermiso(nombre: string): bool
+ {abstract} Agregar(permiso: Permiso)
+ {abstract} Quitar(permiso: Permiso)
+ {abstract} ObtenerHijos(): List<Permiso>
}
class PermisoSimple
class PermisoCompuesto {
- hijos: List<Permiso>
}
FormGestionRoles --> RolesBLL
FormGestionRoles --> EstadoRoles
RolesBLL --> RolesDAL
RolesBLL ..> MatrizRoles
RolesBLL ..> SesionServicio
RolesDAL ..> EstadoRoles
SesionServicio --> "0..1" Usuario : UsuarioActual
EstadoRoles --> "0..*" Rol
EstadoRoles --> "0..*" Permiso
EstadoRoles --> "0..*" Usuario
Usuario --> "0..*" Rol
Rol --> "0..*" Rol : roles anidados
Rol --> "0..*" Permiso
Permiso <|-- PermisoSimple
Permiso <|-- PermisoCompuesto
PermisoCompuesto --> "0..*" Permiso : hijos
note bottom of FormGestionRoles : CrearRol existe pero no tiene acceso en la UI actual.
note bottom of Rol : Rol no hereda de Permiso.
@enduml
''',encoding='utf-8')
with zipfile.ZipFile(BASE/'Documentacion_Roles.zip','w',zipfile.ZIP_DEFLATED) as z:
    for p in sorted(BASE.iterdir()):
        if p.suffix in ('.txt','.puml','.png','.svg'):z.write(p,p.name)
    for p in sorted((BASE.parent/'Diagramas').iterdir()):
        if p.suffix in ('.txt','.puml','.png','.svg'):z.write(p,'Detalles_asignacion/'+p.name)
print('Documentación generada en', BASE)
