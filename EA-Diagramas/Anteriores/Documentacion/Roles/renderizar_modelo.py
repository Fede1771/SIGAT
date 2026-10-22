from pathlib import Path
import zipfile

base = Path(__file__).resolve().parent
source = (base.parent / 'Diagramas' / 'renderizar.py').read_text(encoding='utf-8')
source = source.split("for path in sorted(BASE.glob('*.puml'))")[0]
scope = {'__file__': str(__file__)}
exec(compile(source, 'renderizador', 'exec'), scope)
scope['render'](base / '05_Secuencia_modelo_profesor.puml')
with zipfile.ZipFile(base / 'Secuencia_modelo_profesor.zip', 'w', zipfile.ZIP_DEFLATED) as archive:
    for ext in ('puml', 'png', 'svg'):
        path = base / ('05_Secuencia_modelo_profesor.' + ext)
        archive.write(path, path.name)
