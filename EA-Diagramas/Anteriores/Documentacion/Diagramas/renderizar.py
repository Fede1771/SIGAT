from pathlib import Path
from html import escape
import re
from PIL import Image, ImageDraw, ImageFont

BASE = Path(__file__).resolve().parent
W = 2460
INK = '#20262e'
GRAY = '#757f8a'
GOLD = '#e6ae19'
BLUE = '#dae8fc'
FONT = 'C:/Windows/Fonts/segoeui.ttf'
BOLD = 'C:/Windows/Fonts/segoeuib.ttf'

def render(path):
    lines = path.read_text(encoding='utf-8').splitlines()
    participants = []
    title = path.stem
    start = 0
    for i, line in enumerate(lines):
        if line.startswith('title '): title = line[6:]
        match = re.match(r'(actor|participant) (?:(?:"(.*?)")|(\w+)) as (\w+)', line)
        if match:
            kind, quoted, bare, alias = match.groups()
            participants.append((alias, quoted or bare, kind))
            start = i + 1
    xs = {p[0]: 150 + i * (W-300)/(len(participants)-1) for i, p in enumerate(participants)}
    canvas = Image.new('RGB', (W, 13000), 'white')
    draw = ImageDraw.Draw(canvas)
    svg = []
    font = ImageFont.truetype(FONT, 22)
    small = ImageFont.truetype(FONT, 20)
    bold = ImageFont.truetype(BOLD, 22)
    heading = ImageFont.truetype(BOLD, 34)

    def rect(x,y,w,h,fill='white',stroke=None,width=1):
        draw.rectangle((x,y,x+w,y+h), fill=fill, outline=stroke, width=width)
        svg.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{fill}" stroke="{stroke or "none"}" stroke-width="{width}"/>')
    def line(x1,y1,x2,y2,color=GRAY,width=2,dashed=False):
        if dashed:
            length=((x2-x1)**2+(y2-y1)**2)**.5
            for d in range(0,int(length),10):
                a=d/max(length,1); b=min(d+5,length)/max(length,1)
                draw.line((x1+(x2-x1)*a,y1+(y2-y1)*a,x1+(x2-x1)*b,y1+(y2-y1)*b),fill=color,width=width)
        else: draw.line((x1,y1,x2,y2),fill=color,width=width)
        svg.append(f'<line x1="{x1}" y1="{y1}" x2="{x2}" y2="{y2}" stroke="{color}" stroke-width="{width}"'+(' stroke-dasharray="5 5"' if dashed else '')+'/>')
    def label(text,x,y,maxwidth=1500,face=font,fill=INK):
        result=[]
        for para in text.replace('\\n','\n').split('\n'):
            current=''
            for word in para.split():
                candidate=(current+' '+word).strip()
                if current and draw.textlength(candidate,font=face)>maxwidth:
                    result.append(current); current=word
                else: current=candidate
            result.append(current)
        h=len(result)*30
        for i,t in enumerate(result):
            tw=draw.textlength(t,font=face)
            draw.text((x-tw/2,y+i*30),t,font=face,fill=fill)
            svg.append(f'<text x="{x}" y="{y+i*30+23}" text-anchor="middle" fill="{fill}" font-family="Segoe UI, sans-serif" font-size="{face.size}"'+(' font-weight="bold"' if face in [bold,heading] else '')+f'>{escape(t)}</text>')
        return h
    def arrow(a,b,text,y,dashed=False):
        x1,x2=xs[a],xs[b]
        h=max(30,30*(1+len(text)//65))
        center=max(420,min(W-420,(x1+x2)/2 if a!=b else x1+170))
        label(text,center,y,750 if a==b else 1000)
        ay=y+h+14
        if a==b:
            line(x1,ay,x1+65,ay,dashed=dashed)
            line(x1+65,ay,x1+65,ay+20,dashed=dashed)
            line(x1+65,ay+20,x1,ay+20,dashed=dashed)
            x2=x1; ay+=20; direction=-1
        else:
            line(x1,ay,x2,ay,dashed=dashed); direction=1 if x2>x1 else -1
        points=[(x2,ay),(x2-12*direction,ay-6),(x2-12*direction,ay+6)]
        draw.polygon(points,fill=GRAY)
        svg.append('<polygon points="'+' '.join(f'{x},{yy}' for x,yy in points)+f'" fill="{GRAY}"/>')
        return ay+26

    label(title,W/2,24,2200,heading)
    label('Diagrama de secuencia · correspondencia con el código actual',W/2,74,2200,small)
    header_y=125
    # Lifelines go behind all labels and fragments.
    lifelines=[]
    for alias,name,kind in participants:
        x=xs[alias]
        if kind=='actor':
            draw.ellipse((x-10,header_y,x+10,header_y+20),fill=BLUE,outline='#8ab2e2',width=2)
            svg.append(f'<circle cx="{x}" cy="{header_y+10}" r="10" fill="{BLUE}" stroke="#8ab2e2"/>')
            for p in [(x,header_y+20,x,header_y+48),(x-16,header_y+29,x+16,header_y+29),(x,header_y+48,x-14,header_y+65),(x,header_y+48,x+14,header_y+65)]: line(*p,color='#8ab2e2')
            label(name,x,header_y+70,180,small)
        else:
            rect(x-102,header_y,204,82,BLUE,'#8ab2e2',2)
            label(name.replace(':', ':\\n'),x,header_y+10,192,small)
        lifelines.append(f'<line x1="{x}" y1="235" x2="{x}" y2="HEIGHT" stroke="#aeb6bf" stroke-dasharray="6 5"/>')
        for yy in range(235,13000,11): draw.line((x,yy,x,yy+5),fill='#b7bec7',width=1)
    y=245; stack=[]; number=0
    for raw in lines[start:]:
        if raw.startswith('@end'): break
        if raw.startswith('note over ') or raw.startswith('ref over '):
            isref=raw.startswith('ref ')
            text=raw.split(': ',1)[1]
            h=30*sum(max(1,(len(t)+100)//101) for t in text.split('\\n'))+30
            # Explanatory notes are full width; references identify a linked detail.
            rect(150,y,W-230,h,'#fff9e8' if not isref else 'white',GOLD,2)
            if isref: label('ref',180,y+4,60,bold)
            actual=label(text,W/2+35,y+12,W-440,small)
            y+=max(h,actual+24)+18
            continue
        match=re.match(r'(alt|break|loop|opt|group) (.*)',raw)
        if match:
            kind,guard=match.groups(); inset=18*len(stack)
            x=38+inset; right=W-35-inset
            stack.append((x,right,y,kind))
            rect(x,y,80,32,'#fff4cf',GOLD,2)
            label(kind,x+40,y+1,76,bold)
            label(guard,(x+right)/2,y+3,right-x-210,small, '#805b00')
            y+=55; continue
        if raw.startswith('else '):
            x,right,_,_=stack[-1]
            line(x,y,right,y,GOLD,2,True)
            label(raw[5:],(x+right)/2,y+8,right-x-100,small,'#805b00')
            y+=65;continue
        if raw=='end':
            if stack:
                x,right,top,kind=stack.pop()
                for p in [(x,top,x,y),(right,top,right,y),(x,y,right,y),(x+80,top,right,top)]: line(*p,GOLD,2)
                y+=23
            continue
        match=re.match(r'(\w+) (-+>) (\w+): (.*)',raw)
        if match:
            a,mark,b,text=match.groups()
            number+=1
            y=arrow(a,b,f'{number}. {text}',y,len(mark)>2)
    H=y+72
    canvas.crop((0,0,W,H)).save(path.with_suffix('.png'))
    svg_base=[f'<svg xmlns="http://www.w3.org/2000/svg" width="{W}" height="{H}" viewBox="0 0 {W} {H}">','<rect width="100%" height="100%" fill="white"/>']
    path.with_suffix('.svg').write_text('\n'.join(svg_base+[l.replace('HEIGHT',str(H-32)) for l in lifelines]+svg+['</svg>']),encoding='utf-8')
    print(f'{path.stem}: {W} x {H}')

for path in sorted(BASE.glob('*.puml')): render(path)

import zipfile
with zipfile.ZipFile(BASE/'Diagramas_Gestion_Roles.zip','w',zipfile.ZIP_DEFLATED) as archive:
    for ext in ('*.png','*.svg','*.puml','Correspondencia_codigo.txt'):
        for path in sorted(BASE.glob(ext)): archive.write(path,path.name)
