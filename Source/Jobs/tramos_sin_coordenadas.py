"""
tramos_sin_coordenadas.py
=========================
Muestra los tramos que aparecen en rutas_estado.json (scraper)
pero NO tienen correspondencia en tramos_db.json (base de datos).
Estos son tramos nuevos desde 2020 sin coordenadas.
"""

import json
import re
import unicodedata
from pathlib import Path

def quitar_acentos(s):
    s = unicodedata.normalize("NFD", s)
    return "".join(c for c in s if unicodedata.category(c) != "Mn")

def normalizar(s):
    s = s.lower().strip()
    s = quitar_acentos(s)
    abrev = [
        (r"emp\.?\s*", "empalme "),
        (r"lte\.?\s*", "limite "),
        (r"acc\.?\s*", "acceso "),
        (r"n[º°]\s*",  ""),
        (r"rn\.?\s*",  "ruta nacional "),
        (r"rp\.?\s*",  "ruta provincial "),
        (r"km\.?\s*[\d\.,]+", ""),
    ]
    for patron, reemplazo in abrev:
        s = re.sub(patron, reemplazo, s)
    s = re.sub(r"[^\w\s]", " ", s)
    s = re.sub(r"\b\d+\b", "", s)
    s = re.sub(r"\s+", " ", s).strip()
    return s

def palabras(s):
    return set(w for w in normalizar(s).split() if len(w) > 3)

def tiene_match(tramo_scraper, tramos_db_ruta):
    ps = palabras(tramo_scraper)
    if not ps:
        return False
    for t in tramos_db_ruta:
        pd = palabras(t.get("TramoNormalizado", ""))
        if pd and len(ps & pd) >= 1:
            return True
    return False

def main():
    estado_data = json.loads(Path("rutas_estado.json").read_text(encoding="utf-8"))
    db_data     = json.loads(Path("tramos_db.json").read_text(encoding="utf-8"))

    # Agrupar DB por número de ruta
    RUTAS = {
        1:"3", 2:"5", 3:"7", 4:"8", 5:"9", 6:"11", 7:"12", 8:"14",
        9:"16", 10:"18", 11:"19", 12:"20", 13:"22", 14:"23", 15:"25",
        16:"26", 17:"33", 18:"34", 19:"35", 20:"36", 21:"38", 22:"40",
        23:"50", 24:"51", 25:"52", 26:"60", 27:"64", 28:"65", 29:"66",
        30:"68", 31:"73", 32:"74", 33:"75", 34:"76", 35:"77", 36:"78",
        37:"79", 38:"81", 39:"86", 40:"89", 41:"95", 42:"98", 43:"101",
        44:"105", 45:"117", 46:"118", 47:"119", 48:"120", 49:"121",
        50:"122", 51:"123", 52:"127", 53:"130", 54:"131", 55:"135",
        56:"136", 57:"141", 58:"142", 59:"143", 60:"144", 61:"146",
        62:"147", 63:"148", 64:"149", 65:"150", 66:"151", 67:"152",
        68:"153", 69:"154", 70:"157", 71:"158", 72:"168", 73:"173",
        74:"174", 75:"175", 76:"178", 77:"188", 78:"193", 79:"226",
        80:"228", 81:"229", 82:"231", 83:"232", 84:"234", 85:"237",
        86:"242", 87:"249", 88:"250", 89:"251", 90:"252", 91:"259",
        92:"260", 93:"281", 94:"288", 95:"293",
    }

    db_por_ruta = {}
    for t in db_data:
        ruta = RUTAS.get(t.get("IdRuta"), str(t.get("IdRuta")))
        db_por_ruta.setdefault(ruta, []).append(t)

    rutas_estado = estado_data.get("rutas", {})

    print(f"\nTramos en el scraper SIN coordenadas en la DB:\n")
    print(f"{'─'*70}")

    total_sin_coords = 0

    for ruta, tramos_sc in sorted(rutas_estado.items(), key=lambda x: x[0].zfill(5)):
        tramos_db = db_por_ruta.get(ruta, [])
        sin_match = []

        for t in tramos_sc:
            if not tiene_match(t["tramo"], tramos_db):
                sin_match.append(t)

        if sin_match:
            print(f"\nRN {ruta} ({len(sin_match)} sin coordenadas):")
            for t in sin_match:
                print(f"  [{t['estado']}] {t['tramo']}")
            total_sin_coords += len(sin_match)

    print(f"\n{'─'*70}")
    print(f"Total tramos sin coordenadas: {total_sin_coords}")

if __name__ == "__main__":
    main()