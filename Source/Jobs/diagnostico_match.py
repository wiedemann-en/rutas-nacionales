"""
diagnostico_match.py
====================
Muestra los tramos de tramos.json que quedaron sin estado (default),
comparándolos con los tramos disponibles en rutas_estado.json para
cada ruta, ayudando a identificar si el problema es de matching
o si son tramos nuevos no presentes en el scraper.

Uso:
    python diagnostico_match.py
"""

import json
from pathlib import Path

def cargar(path):
    return json.loads(Path(path).read_text(encoding="utf-8"))

def main():
    tramos   = cargar("tramos.json")["tramos"]
    estados  = cargar("rutas_estado.json")["rutas"]

    sin_estado = [t for t in tramos if t["estado"] == "default"]

    print(f"\nTotal tramos sin estado: {len(sin_estado)}\n")
    print(f"{'─'*80}")

    rutas_sin_datos = set()

    for t in sin_estado:
        ruta = t["ruta"]
        tramos_scraper = estados.get(ruta, [])

        print(f"\nRN {ruta} | {t['provincia']}")
        print(f"  DB:      {t['tramo']}")

        if not tramos_scraper:
            print(f"  Scraper: ⚠️  Ruta {ruta} NO existe en rutas_estado.json")
            rutas_sin_datos.add(ruta)
        else:
            print(f"  Scraper: {len(tramos_scraper)} tramos disponibles:")
            for ts in tramos_scraper:
                print(f"           • {ts['tramo']} → {ts['estado']}")

    print(f"\n{'─'*80}")
    if rutas_sin_datos:
        print(f"\nRutas enteras sin datos en el scraper ({len(rutas_sin_datos)}):")
        for r in sorted(rutas_sin_datos):
            print(f"  RN {r}")
    print()

if __name__ == "__main__":
    main()