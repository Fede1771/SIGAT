import json,hashlib,re,html
from pathlib import Path
from collections import Counter
root=Path.cwd(); out=root/'entregables/modelo_actualizado_20261021'
def read(name):return json.loads((out/'fuentes'/name).read_text(encoding='utf-8-sig'))
src=read('codigo_actual.json'); model=read('modelo.json'); native=read('inspeccion_eap.json'); views=read('exportaciones.json'); schema=read('esquema_sql_actual.json')
checks={}; checks['fuentes_sin_cambios']=all(hashlib.sha256((root/f['path']).read_bytes()).hexdigest()==f['sha256'] for f in src['sources'])
checks['instalador_sin_cambios']=hashlib.sha256((root/schema['source']).read_bytes()).hexdigest()==schema['sha256']
checks['44_diagramas_nativos']=len(native['diagrams'])==44
checks['44_imagenes_exportadas']=len(list((out/'diagramas').glob('*.png')))==44
objects={o['Object_ID']:o for o in native['objects']}; pkgs={p['Package_ID']:p['Name'] for p in native['packages']}
actual={(pkgs.get(o['Package_ID']),o['Name']) for o in objects.values() if pkgs.get(o['Package_ID'],'').startswith('SIGAT.') and o['Object_Type'] != 'Note'}
expected={(t['layer'],t['name']) for t in src['types']}
checks['63_tipos_del_codigo_representados']=expected==actual and len(expected)==63
checks['conectores_sin_extremos_huerfanos']=all(c['Start_Object_ID'] in objects and c['End_Object_ID'] in objects for c in native['connectors'])
checks['sin_Perfil_obsoleto']=not any(o['Name']=='Perfil' for o in objects.values())
tables={o['Name']:o for o in objects.values() if o['Stereotype']=='table'}
checks['15_tablas_instalador_y_un_historico']=set(tables)=={c['Tabla'] for c in schema['columns']}|{'Bitacora_Historico'}
cols={name:{m['Name'] for m in native['members'] if m['Object_ID']==o['Object_ID']} for name,o in tables.items()}
checks['columnas_en_eap_coinciden_con_esquema']=all(cols[name]=={c['Columna'] for c in schema['columns'] if c['Tabla']==name} for name in tables if name!='Bitacora_Historico')
checks['DVH_en_Usuarios_y_UsuarioRol']=all('DigitoVerificador' in cols[t] for t in ['Usuarios','UsuarioRol'])
checks['sin_IdPerfil']=not any(m['Name']=='IdPerfil' for m in native['members'])
# Independently compare CREATE TABLE and ALTER ADD columns of current installer.
sql=(root/schema['source']).read_text(encoding='utf-8-sig'); parsed={}
for hit in re.finditer(r'CREATE TABLE\s+(?:\[dbo\]\.\[([^\]]+)\]|dbo\.(\w+))\s*\(',sql,re.I):
 name=hit[1] or hit[2]; start=hit.end(); depth=1;i=start
 while depth and i<len(sql):
  if sql[i]=='(':depth+=1
  elif sql[i]==')':depth-=1
  i+=1
 body=sql[start:i-1]
 parsed[name]={m[1] or m[2] for m in re.finditer(r'(?:^|,|\n)\s*(?:\[(\w+)\]|(\w+))\s+\[?(?:int|bigint|varchar|nvarchar|char|bit|datetime|uniqueidentifier)\b',body,re.I)}
for t,c in re.findall(r'ALTER TABLE dbo\.(\w+) ADD (\w+)\s+char\(64\)',sql,re.I):parsed[t].add(c)
checks['columnas_contrastadas_con_SQL_vigente']=all(parsed[t]==cols[t] for t in parsed) and len(parsed)==15
report={'fecha':'2026-10-21','archivos_CSharp':len(src['sources']),'tipos':len(expected),'tipos_por_capa':dict(Counter(t['layer'] for t in src['types'])),'diagramas':len(views),'tablas_instalador':15,'tablas_creadas_al_archivar':1,'verificaciones':checks,'todos_correctos':all(checks.values()),'alcance':'Validación documental y estructural; no ejecuta la aplicación ni cambia SQL Server.'}
(out/'fuentes/validacion.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
assert all(checks.values()),json.dumps(checks,indent=2)
items='\n'.join(f"| {v['id'].split('_')[0]} | {v['title']} | [PNG](diagramas/{v['id']}.png) |" for v in views)
readme=f'''# SIGAT · Modelo actualizado para Enterprise Architect

Reconstruido el **21/10/2026** a partir del código vigente: **{len(src['sources'])} archivos C#, {len(expected)} tipos en cinco capas, 15 tablas del instalador y una tabla creada al archivar**. Contiene **44 diagramas nativos y editables**.

## Abrir el proyecto

1. Abrir `SIGAT_Modelo_Actualizado.EAP` con Enterprise Architect.
2. Desplegar el modelo **SIGAT - Código vigente 2026-10-21** en el explorador de proyectos.
3. Usar **G06 Clases por capa**, **G07 Modelo de datos** y **Módulos y recorridos implementados**.
4. Para consultar sin Enterprise Architect, abrir [index.html](index.html). Las imágenes permiten ampliar cada diagrama.

La entrega se generó en una copia independiente. Los proyectos anteriores se conservaron; no se modificó código fuente, SQL ni datos.

## Qué mirar primero

- **11 y 12:** diagramas de clases de login y cierre de sesión, actualizados respecto de las dos imágenes originales.
- **24, 25 y 26:** secuencias de apertura, autenticación y cierre de sesión.
- **02–06:** integración de clases por cada capa: BE, BLL, DAL, SERVICIOS y UI.
- **07–10 y 44:** tablas y relaciones persistentes, separando seguridad, idiomas y soporte técnico.
- **14, 15 y 17:** Composite de permisos/familias, Observer de idiomas y control de integridad.
- **19–23:** casos de uso de las funciones implementadas.
- **43:** alcance pendiente de T06b; no se presenta como funcionalidad realizada.

## Criterios del modelo

- Las clases reutilizan elementos del mismo modelo. Se muestran miembros principales para mantener la lectura; las notas de cada elemento incluyen su archivo de origen y las firmas extraídas del código. Los gráficos no son una copia de todos los controles generados por el diseñador WinForms.
- Los diagramas de secuencia describen el recorrido principal. Las validaciones, errores y alternativas se explican mediante notas; no se representan todas las ramas como fragmentos UML `alt`.
- DVH/DVV se aplica a **Usuarios y UsuarioRol**. Las columnas antiguas de Bitacora y Traduccion se conservan en el modelo físico como legado; no indican protección activa de esas tablas.
- La bitácora guarda eventos y cifra su detalle con AES. **Bitacora_Historico** se crea al archivar y conserva los identificadores originales. No equivale a un historial de versiones de una entidad.
- El control de integridad puede bloquear el inicio antes del login. Se documentan preparación inicial, respaldo comprobado, recuperación y recálculo con aceptación de pérdida.
- Roles y familias reflejan la implementación actual. No se agregan familias de ejemplo ni pantallas de creación de roles que no estén disponibles en la UI.
- SIGAT apunta a administrar activos tecnológicos; los módulos de activos, movimientos y mantenimiento siguen siendo alcance futuro y no se incluyen como implementación existente.

**T06b pendiente:** faltan conservación de versiones, consulta de quién/cuándo/qué cambió y recomposición de un estado anterior. Este modelo documenta el estado actual, no acredita por sí solo el cumplimiento completo de la tercera entrega.

## Catálogo

| N.º | Diagrama | Vista previa |
| --- | --- | --- |
{items}

## Trazabilidad y comprobación

`fuentes/codigo_actual.json` conserva los tipos, contratos y SHA-256 de los archivos revisados. `fuentes/esquema_sql_actual.json` identifica el instalador; sus columnas se contrastaron con el SQL vigente. `fuentes/modelo.json` contiene la especificación documental y `fuentes/exportaciones.json` relaciona cada PNG con su diagrama nativo.

[Resultado de validación](fuentes/validacion.json): tipos por capa, tablas, columnas, extremos de relaciones, cantidad de diagramas y comprobación de que las fuentes no cambiaron. No se ejecutaron restauraciones ni operaciones sobre la base de datos.
'''
(out/'README.md').write_text(readme,encoding='utf-8')
cards='\n'.join(f'<article><a href="diagramas/{v["id"]}.png"><img loading="lazy" src="diagramas/{v["id"]}.png" alt="{html.escape(v["title"])}"></a><p>{html.escape(v["id"].split("_")[0])} · {html.escape(v["title"])}</p></article>' for v in views)
(out/'index.html').write_text('''<!doctype html><html lang="es"><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>SIGAT · Modelo actualizado</title><style>body{margin:0;font:16px system-ui;background:#f3f6fa;color:#172a40}header,main{max-width:1300px;margin:auto;padding:32px}h1{margin:0 0 12px}a{color:#165bb5}nav{display:flex;gap:24px;margin:24px 0}.info{padding:18px;border-left:4px solid #2563a6;background:#e4eefb}.grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(340px,1fr));gap:20px}article{padding:16px;background:white;border:1px solid #d9e0e9;border-radius:8px}img{width:100%;height:230px;object-fit:contain;background:white}p{line-height:1.6}article p{font-weight:600;margin-bottom:0}</style><header><h1>SIGAT · Modelo actualizado</h1><p>21/10/2026 · 44 diagramas editables · cinco capas · código vigente</p><nav><a href="SIGAT_Modelo_Actualizado.EAP">Proyecto de Enterprise Architect</a><a href="README.md">Guía y alcance</a><a href="fuentes/validacion.json">Validación</a></nav><p class="info">Los diagramas 11 y 12 actualizan login y cierre de sesión. El resto documenta clases por capa, tablas, casos de uso y secuencias actuales. T06b sigue pendiente de implementación. Pulsá una imagen para verla completa.</p></header><main class="grid">'''+cards+'</main></html>',encoding='utf-8')
print(json.dumps(report,ensure_ascii=False,indent=2))


