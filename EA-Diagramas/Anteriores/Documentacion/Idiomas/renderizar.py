from pathlib import Path
from html import escape
import re
import zipfile
from PIL import Image, ImageDraw, ImageFont

BASE = Path(__file__).resolve().parent
PATH = BASE / '01_Cambiar_idioma.puml'
W = 3300
font = ImageFont.truetype('C:/Windows/Fonts/segoeui.ttf', 23)
bold = ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf', 24)
title_font = ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf', 32)
canvas = Image.new('RGB', (W, 10000), '#ffffff')
draw = ImageDraw.Draw(canvas)
elements = []
text_layers = []
activation_elements = []

def line(x1,y1,x2,y2,color='#333333',dashed=False):
    if dashed:
        length=max(1,((x2-x1)**2+(y2-y1)**2)**.5)
        for n in range(0,int(length),12):
            a=n/length;b=min(n+6,length)/length
            draw.line((x1+(x2-x1)*a,y1+(y2-y1)*a,x1+(x2-x1)*b,y1+(y2-y1)*b),fill=color,width=2)
    else: draw.line((x1,y1,x2,y2),fill=color,width=2)
    elements.append(f'<line x1="{x1}" y1="{y1}" x2="{x2}" y2="{y2}" stroke="{color}" stroke-width="2"'+(' stroke-dasharray="6 6"' if dashed else '')+'/>')

def rect(x,y,w,h,fill='white',border='#8ab2e2'):
    draw.rectangle((x,y,x+w,y+h),fill=fill,outline=border,width=2)
    elements.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{fill}" stroke="{border}" stroke-width="2"/>')

def text(x,y,value,face=font):
    half=draw.textlength(value,font=face)/2
    x=max(half+24,min(W-half-24,x))
    text_layers.append((x,y,value,face))
    draw.text((x-draw.textlength(value,font=face)/2,y),value,font=face,fill='#20262e')
    elements.append(f'<text x="{x}" y="{y+face.size}" text-anchor="middle" font-family="Segoe UI" font-size="{face.size}" font-weight="{700 if face!=font else 400}">{escape(value)}</text>')

lines=PATH.read_text(encoding='utf-8').splitlines()
participants=[]
for raw in lines:
    m=re.match(r'(actor|participant|database) (?:"([^"]+)"|(\w+)) as (\w+)',raw)
    if m:participants.append((m[4],m[2] or m[3],m[1]))
xs={alias:200+i*(W-400)/(len(participants)-1) for i,(alias,_,_) in enumerate(participants)}
text(W/2,20,'DIAGRAMA DE SECUENCIA — CAMBIAR IDIOMA DE LA INTERFAZ',title_font)
for alias,name,kind in participants:
    x=xs[alias]
    if kind=='actor':
        draw.ellipse((x-12,75,x+12,99),outline='#666666',width=2)
        elements.append(f'<circle cx="{x}" cy="87" r="12" fill="none" stroke="#666666"/>')
        for segment in [(x,99,x,131),(x-20,111,x+20,111),(x,131,x-18,153),(x,131,x+18,153)]:line(*segment)
        text(x,159,name)
    else:
        fill='#dae8fc'
        border='#8ab2e2'
        rect(x-180,91,360,53,fill,border)
        text(x,104,name,bold)

lifelines=[]
for x in xs.values():
    for yy in range(200,10000,12):draw.line((x,yy,x,yy+6),fill='#a5bbd8',width=1)
    lifelines.append(f'<line x1="{x}" y1="200" x2="{x}" y2="HEIGHT" stroke="#a5bbd8" stroke-dasharray="6 6"/>')
y=215;stack=[];active={};count=0
for raw in lines:
    if raw.startswith('activate '):
        alias=raw.split()[1];active.setdefault(alias,[]).append(y-16);continue
    if raw.startswith('deactivate '):
        alias=raw.split()[1]
        if active.get(alias):
            top=active[alias].pop();rect(xs[alias]-7,top,14,max(18,y-top-12))
            activation_elements.append(elements.pop())
        continue
    if raw.startswith(('note over ','ref over ')):
        label=raw.split(': ',1)[1]
        rect(55,y,W-110,40,'#fffaf0','#e6ae19')
        text(W/2,y+5,('ref — ' if raw.startswith('ref ') else '')+label)
        y+=50;continue
    m=re.match(r'(alt|opt|loop) (.*)',raw)
    if m:
        inset=len(stack)*11;left=32+inset;right=W-32-inset
        stack.append((left,right,y))
        rect(left,y,60,32,'#fff0d4','#e6ae19')
        text(left+30,y+1,m[1],bold);text((left+right)/2,y+1,m[2]);y+=43;continue
    if raw.startswith('else '):
        left,right,_=stack[-1];line(left,y,right,y,'#e6ae19',True)
        text((left+right)/2,y+6,raw[5:]);y+=42;continue
    if raw=='end':
        left,right,top=stack.pop()
        for seg in [(left,top,left,y),(right,top,right,y),(left,y,right,y),(left+60,top,right,top)]:line(*seg,color='#e6ae19',dashed=True)
        y+=12;continue
    m=re.match(r'(\w+) (-+>) (\w+): (.*)',raw)
    if m:
        a,mark,b,label=m.groups();count+=1
        label=re.sub(r'^1\. ', '', label)
        x1,x2=xs[a],xs[b]
        center=(x1+x2)/2 if a!=b else min(W-440,x1+200)
        text(center,y,f'{count}. {label}')
        ay=y+31
        if a==b:
            line(x1,ay,x1+40,ay);line(x1+40,ay,x1+40,ay+14);line(x1+40,ay+14,x1,ay+14);ay+=14
        else:line(x1,ay,x2,ay,dashed=len(mark)>2)
        direction=1 if x2>x1 else -1
        points=[(x2,ay),(x2-10*direction,ay-5),(x2-10*direction,ay+5)]
        draw.polygon(points,fill='#333333')
        elements.append('<polygon points="'+' '.join(f'{x},{yy}' for x,yy in points)+'" fill="#333333"/>')
        y=ay+13
assert not stack, 'Bloques pendientes'
assert all(not v for v in active.values()), 'Activaciones pendientes'
H=y+35
for x,yy,value,face in text_layers:
    draw.text((x-draw.textlength(value,font=face)/2,yy),value,font=face,fill='#20262e')
canvas.crop((0,0,W,H)).save(PATH.with_suffix('.png'))
PATH.with_suffix('.svg').write_text('\n'.join([f'<svg xmlns="http://www.w3.org/2000/svg" width="{W}" height="{H}" viewBox="0 0 {W} {H}">','<rect width="100%" height="100%" fill="#ffffff"/>']+[s.replace('HEIGHT',str(H-20)) for s in lifelines]+activation_elements+elements+['</svg>']),encoding='utf-8')
with zipfile.ZipFile(BASE/'Diagrama_Idiomas.zip','w',zipfile.ZIP_DEFLATED) as archive:
    for ext in ('puml','png','svg'):
        p=PATH.with_suffix('.'+ext);archive.write(p,p.name)
    archive.write(BASE/'Correspondencia_codigo.txt','Correspondencia_codigo.txt')
print(f'Diagrama generado: {W} x {H}; bloques y activaciones verificados.')
