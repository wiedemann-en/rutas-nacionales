import json

rutas = ['3','35','40','142','143','153','174','193','281']
data  = json.load(open('rutas_estado.json', encoding='utf-8'))

for r in rutas:
    tramos = data['rutas'].get(r, [])
    print(f'\nRN {r} ({len(tramos)} tramos):')
    for t in tramos:
        print(f'  [{t["estado"]}] {t["tramo"]}')