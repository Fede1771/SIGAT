from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
from html import escape
import textwrap

OUT = Path(__file__).resolve().parents[2] / 'entregables' / 'login_corregido_20261014'
OUT.mkdir(parents=True, exist_ok=True)
W, H = 2600, 1790
im = Image.new('RGB', (W, H), '#ffffff')
d = ImageDraw.Draw(im)
svg = [f'<svg xmlns="http://www.w3.org/2000/svg" width="{W}" height="{H}" viewBox="0 0 {W} {H}">', '<rect width="100%" height="100%" fill="white"/>']
font_path = 'C:/Windows/Fonts/arial.ttf'
bold_path = 'C:/Windows/Fonts/arialbd.ttf'
def text(x, y, s, size=19, color='#202020', center=False, bold=False):
    f = ImageFont.truetype(bold_path if bold else font_path, size)
    for i, line in enumerate(s.split('\n')):
        yy = y + i * (size + 5)
        xx = x - d.textlength(line, font=f)/2 if center else x
        box = d.textbbox((0, 0), line, font=f)
        d.text((xx, yy-box[1]), line, fill=color, font=f)
        svg.append(f'<text x="{x}" y="{yy+size}" text-anchor="{"middle" if center else "start"}" font-family="Arial" font-size="{size}" font-weight="{"bold" if bold else "normal"}" fill="{color}">{escape(line)}</text>')
def line(x1,y1,x2,y2,color='#333333',dash=False,width=2):
    if dash:
        length=((x2-x1)**2+(y2-y1)**2)**.5
        for start in range(0,int(length),11):
            end=min(start+6,length)
            d.line((x1+(x2-x1)*start/length,y1+(y2-y1)*start/length,x1+(x2-x1)*end/length,y1+(y2-y1)*end/length),fill=color,width=width)
    else:d.line((x1,y1,x2,y2),fill=color,width=width)
    svg.append(f'<line x1="{x1}" y1="{y1}" x2="{x2}" y2="{y2}" stroke="{color}" stroke-width="{width}"'+(' stroke-dasharray="6 5"' if dash else '')+'/>')
def rect(x,y,w,h,fill='white',stroke='#7093c7'):
    d.rectangle((x,y,x+w,y+h),fill=fill,outline=stroke,width=2)
    svg.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{fill}" stroke="{stroke}" stroke-width="2"/>')
def poly(points,fill):
    d.polygon(points,fill=fill)
    svg.append('<polygon points="'+' '.join(f'{x},{y}' for x,y in points)+f'" fill="{fill}"/>')
xs=[75,310,550,790,1030,1270,1510,1750,1990,2230,2470]
def message(a,b,y,label,ret=False):
    x1,x2=xs[a],xs[b]
    if a==b:
        line(x1,y,x1+90,y);line(x1+90,y,x1+90,y+20);line(x1+90,y+20,x1,y+20)
        poly([(x1,y+20),(x1+10,y+15),(x1+10,y+25)],'#333333')
        text(x1+105,y-14,label,size=18)
    else:
        maxchars=max(25,int(abs(x2-x1)/10.1))
        lines=textwrap.wrap(label,maxchars)
        text((x1+x2)/2,y-len(lines)*23-6,'\n'.join(lines),size=18,center=True)
        line(x1,y,x2,y,dash=ret)
        direction=1 if x2>x1 else -1
        if ret:
            line(x2,y,x2-direction*11,y-6);line(x2,y,x2-direction*11,y+6)
        else:poly([(x2,y),(x2-direction*12,y-6),(x2-direction*12,y+6)],'#333333')
def frame(y,h,label,guard,x=45,w=2505):
    line(x,y,x+w,y,'#dc9e32',True);line(x,y+h,x+w,y+h,'#dc9e32',True)
    line(x,y,x,y+h,'#dc9e32',True);line(x+w,y,x+w,y+h,'#dc9e32',True)
    rect(x,y,62,25,'#fff0cd','#dc9e32');text(x+10,y+4,label,17,bold=True)
    text(x+80,y+6,guard,18,'#a65f00')
def ref(y,a,b,label):
    x=xs[a]-70;w=xs[b]-xs[a]+140
    rect(x,y,w,49,'#fffaf0','#dc9e32')
    text(x+8,y+7,'ref',15,bold=True,color='#a65f00')
    text(x+w/2,y+12,label,17,center=True)

text(W/2,15,'DIAGRAMA DE SECUENCIA – LOGIN',28,center=True,bold=True)
line(W/2-258,48,W/2+258,48,'#202020',width=1)
names=['Usuario',':Sistema\n(FrmLogin)',':UsuarioBLL',':IntegridadBLL',':UsuarioDAL',':HashHelper',':RolesBLL',':SesionServicio',':BitacoraBLL',':BitacoraDAL','BD SIGAT']
for i,(x,name) in enumerate(zip(xs,names)):
    line(x,125,x,1750,'#7f9ac0',True,width=1)
    if i==0:
        d.ellipse((x-9,66,x+9,84),fill='#ffe6b6',outline='#333333',width=2)
        svg.append(f'<circle cx="{x}" cy="75" r="9" fill="#ffe6b6" stroke="#333"/>')
        line(x,84,x,104);line(x-15,92,x+15,92);line(x,104,x-13,120);line(x,104,x+13,120)
        text(x,129,name,18,center=True,bold=True)
    elif i==10:
        rect(x-92,69,184,51,'#e3efd9','#70a95a');text(x,87,name,19,center=True,bold=True)
    else:
        rect(x-100,69,200,51,'#ffe7bd' if i==1 else '#dce7f7','#e0a238' if i==1 else '#7396ca')
        text(x,77 if '\n' in name else 86,name,18,center=True,bold=True)

rect(180,158,2330,47,'#f6f8fb','#a2b3cb')
text(1345,171,'Precondición: Program verifica integridad antes de mostrar el login; si falla, abre recuperación y bloquea el acceso.',18,center=True)
rect(xs[1]-5,228,10,1508,'#fff','#7396ca')
rect(xs[2]-5,284,10,1382,'#fff','#7396ca')
message(0,1,247,'1. Ingresar usuario y contraseña')
message(1,2,300,'2. Autenticar(nombreUsuario, passwordPlana, out usuarioValidado)')
message(2,3,359,'3. ExigirIntegridad()')
ref(374,3,5,'IntegridadDAL.Verificar(): comparar DVH y DVV de Bitacora y Traduccion')
frame(478,83,'break','[integridad incorrecta]')
message(3,1,540,'3.2. IntegridadException → recuperación; se interrumpe el login',True)
message(2,4,611,'4. ObtenerPorNombreUsuario(nombreUsuario)')
message(4,10,656,'4.1. SELECT de la cuenta por nombre (parámetro SQL)')
message(10,4,702,'4.2. Datos de la cuenta o sin filas',True)
message(4,2,747,'4.3. Usuario o null',True)
message(2,5,798,'5. ObtenerHashSHA256(passwordPlana)')
message(5,2,844,'5.1. Hash calculado',True)
frame(867,868,'alt','[usuario inexistente o hash distinto]')
ref(901,2,10,'LoginFallidoFactory.Crear(nombreUsuario, "Usuario o contraseña incorrectos.")')
message(2,8,989,'6.1. Registrar(evento); adaptar datos con BitacoraMapper')
message(8,9,1041,'6.1.1. Insertar(eventoConcreto)')
ref(1054,8,10,'IntegridadDAL.Cambiar: cifrado, INSERT, DVH/DVV y commit')
message(2,1,1142,'6.2. CredencialesInvalidas',True)
message(1,0,1195,'6.3. Mostrar "Usuario o contraseña incorrectos"',True)
line(45,1220,2550,1220,'#dc9e32',True)
text(125,1227,'[else: contraseña correcta y usuario inactivo]',18,'#a65f00')
ref(1256,2,10,'LoginFallidoFactory.Crear(nombreUsuario, "El usuario intentó entrar pero está inactivo.")')
message(2,8,1344,'7.1. Registrar(evento); adaptar datos con BitacoraMapper')
message(8,9,1396,'7.1.1. Insertar(eventoConcreto)')
ref(1409,8,10,'IntegridadDAL.Cambiar: cifrado, INSERT, DVH/DVV y commit')
message(2,1,1497,'7.2. UsuarioInactivo',True)
message(1,0,1550,'7.3. Mostrar "El usuario está inactivo"',True)

# La rama válida continúa en una segunda lámina, conservando las mismas líneas de vida.
# Se amplía el lienzo antes de exportar para que las tres alternativas formen un solo fragmento UML.
H2=2330
larger=Image.new('RGB',(W,H2),'white');larger.paste(im,(0,0));im=larger;d=ImageDraw.Draw(im)
svg[0]=f'<svg xmlns="http://www.w3.org/2000/svg" width="{W}" height="{H2}" viewBox="0 0 {W} {H2}">'
# Quitamos el cierre inferior del alt y continuamos las líneas de vida sobre el espacio agregado.
d.rectangle((40,1728,2560,1790),fill='white')
for x in xs:line(x,1728,x,2289,'#7f9ac0',True,width=1)
rect(xs[1]-5,1728,10,540,'#fff','#7396ca')
rect(xs[2]-5,1646,10,455,'#fff','#7396ca')
line(45,1575,2550,1575,'#dc9e32',True);text(125,1582,'[else: contraseña correcta y usuario activo]',18,'#a65f00')
message(2,6,1646,'8. CargarRolesUsuario(usuarioValidado)')
ref(1660,6,10,'RolesDAL.Cargar(); validar catálogo y cargar roles guardados')
message(2,7,1760,'9. ObtenerInstancia().IniciarSesion(usuarioValidado)')
ref(1775,7,10,'Limpiar idioma anterior y conservar usuario con roles')
ref(1845,2,10,'LoginExitosoFactory.Crear(usuarioValidado.NombreUsuario, "Ingreso al sistema.")')
message(2,8,1933,'10. Registrar(eventoExito); adaptar datos con BitacoraMapper')
message(8,9,1985,'10.1. Insertar(eventoConcreto)')
ref(1998,8,10,'IntegridadDAL.Cambiar: cifrado, INSERT, DVH/DVV y commit')
message(2,1,2086,'11. ResultadoLogin.Exito',True)
message(1,1,2158,'12. Hide(); FrmPrincipal.ShowDialog()')
message(1,0,2240,'13. Mostrar pantalla principal',True)
line(45,1735,45,2270,'#dc9e32',True);line(2550,1735,2550,2270,'#dc9e32',True);line(45,2270,2550,2270,'#dc9e32',True)
# El SVG también debe tener un único cierre del fragmento, sin la línea anterior.
svg=[s for s in svg if not ('y1="1735"' in s and 'y2="1735"' in s)]
svg.append('</svg>')
(OUT/'secuencia_login.svg').write_text('\n'.join(svg),encoding='utf-8')
im.save(OUT/'secuencia_login.png',dpi=(220,220))
print(OUT/'secuencia_login.png')
