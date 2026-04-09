"""
generar_tramos.py  v4
=====================
Genera tramos.json usando el scraper como fuente maestra.

Logica:
  - Itera sobre cada tramo del scraper (vialidad.gob.ar)
  - Busca geometria en la DB por ruta + similitud de nombre
  - Si encuentra match: usa esa geometria
  - Si no encuentra: lo loguea en sin_geometria.json para insertar en DB
  - Tramos en DB sin match en el scraper: ignorados

Requisitos:
    pip install requests

Uso:
    python generar_tramos.py
    python generar_tramos.py --sin-osrm
"""

import json
import re
import time
import argparse
import unicodedata
import requests
from pathlib import Path
from datetime import datetime

# CONFIG
OSRM_URL       = "http://router.project-osrm.org/route/v1/driving"
PAUSA_OSRM     = 0.3
MAX_REINTENTOS = 3

CALZADA_MAP = {
    1:"SIN ESPECIFICAR", 2:"TIERRA", 3:"TIERRA", 4:"TIERRA",
    5:"PAVIMENTO", 6:"PAVIMENTO", 7:"RIPIO", 8:"PAVIMENTO",
    9:"PAVIMENTO",  10:"PAVIMENTO", 11:"PAVIMENTO", 12:"PAVIMENTO",
    13:"PAVIMENTO", 14:"PAVIMENTO", 15:"PAVIMENTO", 16:"PAVIMENTO",
    17:"PAVIMENTO", 18:"PAVIMENTO", 19:"PAVIMENTO", 20:"PAVIMENTO",
    21:"PAVIMENTO", 22:"PAVIMENTO", 23:"PAVIMENTO", 24:"PAVIMENTO",
    25:"PAVIMENTO", 26:"PAVIMENTO", 27:"PAVIMENTO", 28:"PAVIMENTO",
    29:"RIPIO",     30:"PAVIMENTO", 31:"PAVIMENTO", 32:"PAVIMENTO",
    33:"PAVIMENTO",
}

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
    92:"260", 93:"281", 94:"288", 95:"293", 96:"1S40", 97:"1V03",
    98:"1v09", 99:"1V38", 100:"11-AO11", 101:"A001", 102:"A005",
    103:"A007", 104:"A008", 105:"A009", 106:"A012", 107:"A014",
    108:"A015", 109:"A016", 110:"A019", 111:"A025", 112:"A026",
    113:"Ex 9", 114:"Ex 34", 115:"IV66", 116:"V146", 117:"R 145",
    118:"Caminos Interiores Campo de Mayo", 119:"Complejo A",
    120:"Complejo B", 121:"Complejo I", 122:"Complejo J",
    123:"Parques Nacionales", 124:"Puentes Riachuelo",
    125:"Region XII - Chile", 126:"S/N",129:"Complejo K",130:"205",
    131:"A017",132:"Au. Ezeiza Cañuelas",133:"Au. Riccheri",
    134:"Av. Jorge Newbery",135:"1V65",136:"1V143",137:"VRN22",138:"4V09"
}

# MATCHING
def quitar_acentos(s):
    s = unicodedata.normalize("NFD", s)
    return "".join(c for c in s if unicodedata.category(c) != "Mn")

def normalizar_texto(s):
    s = s.lower().strip()
    # Normalizar nomenclaturas especiales de rutas
    s = re.sub(r"\ba-(\w)",   r"a\1",     s)   # A-001 → A001
    s = re.sub(r"\bcompl\b",  "complejo", s)   # Compl → Complejo

    abrev = [
        (r"emp\.?\s*",        "empalme "),
        (r"lte\.?\s*",        "limite "),
        (r"acc\.?\s*",        "acceso "),
        (r"int\.?\s*",        "interseccion "),
        (r"cnel\.?\s*",       "coronel "),
        (r"gral\.?\s*",       "general "),
        (r"tte\.?\s*",        "teniente "),
        (r"pte\.?\s*",        "puente "),
        (r"avda\.?\s*",       "avenida "),
        (r"av\.?\s*",         "avenida "),
        (r"rn\.?\s*n[º°]?\s*","ruta nacional "),
        (r"rp\.?\s*n[º°]?\s*","ruta provincial "),
        (r"n[º°]\s*",         ""),
        (r"nro\.?\s*",        ""),
        (r"bs\.?\s*as\.?\s*", "buenos aires "),
        (r"\bcaba\b",         "buenos aires"),
        (r"\bcba\b",          "cordoba"),
        (r"a[º°]\s*",         "arroyo "),
        (r"km\.?\s*[\d\.,]+", ""),
    ]
    for patron, reemplazo in abrev:
        s = re.sub(patron, reemplazo, s)
    s = quitar_acentos(s)
    s = re.sub(r"[^\w\s]", " ", s)
    s = re.sub(r"\b\d+\b",  "", s)
    s = re.sub(r"\s+",      " ", s).strip()
    return s

def palabras_clave(s):
    return set(w for w in normalizar_texto(s).split() if len(w) > 2)

def score_match(nombre_a, nombre_b):
    na = normalizar_texto(nombre_a)
    nb = normalizar_texto(nombre_b)
    pa = palabras_clave(na)
    pb = palabras_clave(nb)
    if not pa or not pb:
        return 0

    interseccion = pa & pb
    union        = pa | pb
    jaccard      = len(interseccion) / len(union) if union else 0

    # Bonus palabras largas (localidades)
    pl_a = set(w for w in pa if len(w) > 4)
    pl_b = set(w for w in pb if len(w) > 4)
    bonus_largo = len(pl_a & pl_b) * 0.2

    # Bonus contenido
    bonus_contenido = 0.1 if (na in nb or nb in na) else 0

    # Bonus extremos del tramo
    def extremos(texto):
        partes = re.split(r"[-/]", texto)
        return [normalizar_texto(p) for p in partes if p.strip()]

    bonus_extremos = 0
    for ea in extremos(nombre_a):
        for eb in extremos(nombre_b):
            pea = palabras_clave(ea)
            peb = palabras_clave(eb)
            if pea and peb and len(pea & peb) >= 1:
                bonus_extremos = 0.3
                break

    return jaccard + bonus_largo + bonus_contenido + bonus_extremos

# Mapeo explícito de nombres del scraper → nombres en DB
RUTA_ALIAS = {
    "A-001":          "A001",
    "A-017":          "A017",
    "Compl A":        "Complejo A",
    "Compl B":        "Complejo B",
    "Compl I":        "Complejo I",
    "Compl J":        "Complejo J",
    "Compl K":        "Complejo K",
    "Vº RN 22":       "VRN22",
}

def normalizar_nombre_ruta(s):
    """Resuelve alias del scraper al nombre equivalente en la DB."""
    return RUTA_ALIAS.get(s, s).strip()

def buscar_en_db(nombre_scraper, tramos_db_ruta, umbral=0.15):
    """
    Dado un nombre de tramo del scraper, busca el mejor match
    en los tramos de la DB para esa ruta.

    Estrategia:
      1. Match exacto por TramoDesnormalizado (mismo texto que el scraper)
      2. Match fuzzy por TramoNormalizado si el paso 1 falla
    """
    nombre_sc_norm = nombre_scraper.strip().lower()

    # Paso 1: match exacto/casi exacto por TramoDesnormalizado
    for t in tramos_db_ruta:
        desnorm = (t.get("TramoDesnormalizado") or "").strip().lower()
        if desnorm and desnorm == nombre_sc_norm:
            return (t, 1.0)

    # Paso 1b: match ignorando acentos y mayúsculas
    nombre_sc_plain = quitar_acentos(nombre_sc_norm)
    for t in tramos_db_ruta:
        desnorm = quitar_acentos((t.get("TramoDesnormalizado") or "").strip().lower())
        if desnorm and desnorm == nombre_sc_plain:
            return (t, 0.99)

    # Paso 2: match fuzzy por TramoNormalizado
    mejor       = None
    mejor_score = 0.0
    for t in tramos_db_ruta:
        s = score_match(nombre_scraper, t.get("TramoNormalizado", ""))
        if s > mejor_score:
            mejor_score = s
            mejor       = t

    return (mejor, mejor_score) if mejor_score >= umbral else (None, 0)

# OSRM
def parsear_coordenadas(coord_str):
    if not coord_str:
        return []
    puntos = []
    for par in coord_str.strip().split("/"):
        par = par.strip()
        if not par:
            continue
        partes = par.split(",")
        if len(partes) == 2:
            try:
                lat = float(partes[0].strip())
                lng = float(partes[1].strip())
                puntos.append([lat, lng])
            except ValueError:
                continue
    return puntos

def osrm_ruta(waypoints):
    coords_str = ";".join(f"{p[1]},{p[0]}" for p in waypoints)
    url    = f"{OSRM_URL}/{coords_str}"
    params = {
        "overview":    "full",
        "geometries":  "geojson",
        "steps":       "false",
        "annotations": "false",
    }
    for intento in range(MAX_REINTENTOS):
        try:
            resp = requests.get(url, params=params, timeout=15)
            if resp.status_code == 200:
                data = resp.json()
                if data.get("code") == "Ok":
                    coords = data["routes"][0]["geometry"]["coordinates"]
                    return [[c[1], c[0]] for c in coords]
        except requests.RequestException as e:
            print(f"      Error OSRM (intento {intento+1}): {e}")
        time.sleep(1)
    return None

# MAIN
def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--db",       default="tramos_db.json")
    parser.add_argument("--estado",   default="rutas_estado.json")
    parser.add_argument("--output",   default="tramos.json")
    parser.add_argument("--log",      default="sin_geometria.json",
                        help="Tramos del scraper sin match en DB")
    parser.add_argument("--sin-osrm", action="store_true")
    args = parser.parse_args()

    print(f"\nCargando datos...")
    db_data     = json.loads(Path(args.db).read_text(encoding="utf-8"))
    estado_data = json.loads(Path(args.estado).read_text(encoding="utf-8"))
    rutas_scraper = estado_data.get("rutas", {})

    # Agrupar DB por NombreRuta (viene directo del JOIN con tabla Ruta)
    db_por_ruta = {}
    for t in db_data:
        ruta = str(t.get("NombreRuta", t.get("IdRuta", ""))).strip()
        db_por_ruta.setdefault(ruta, []).append(t)

    total_scraper = sum(len(v) for v in rutas_scraper.values())
    print(f"  Tramos en scraper: {total_scraper}")
    print(f"  Rutas en scraper:  {len(rutas_scraper)}")
    print(f"  Tramos en DB:      {len(db_data)}\n")

    resultado     = []
    sin_geometria = []  # tramos del scraper sin match en DB
    total         = sum(len(v) for v in rutas_scraper.values())
    procesados    = 0

    for ruta, tramos_sc in sorted(rutas_scraper.items(), key=lambda x: x[0].zfill(5)):
        ruta_norm      = normalizar_nombre_ruta(ruta)
        tramos_db_ruta = db_por_ruta.get(ruta_norm) or db_por_ruta.get(ruta, [])

        for t_sc in tramos_sc:
            procesados += 1
            nombre_sc = t_sc.get("tramo", "")
            estado    = t_sc.get("estado", "default")
            estado_raw= t_sc.get("estado_raw", "")
            obs       = t_sc.get("obs", "")
            actualizado = t_sc.get("actualizado", "")

            print(f"  [{procesados}/{total}] RN {ruta} | {nombre_sc[:50]}...", end=" ")

            # Buscar geometria en DB
            t_db, score = buscar_en_db(nombre_sc, tramos_db_ruta)

            if t_db:
                calzada   = t_db.get("NombreCalzada", "PAVIMENTO") or "PAVIMENTO"
                coord_str = t_db.get("Coordenadas", "") or ""
                waypoints = parsear_coordenadas(coord_str)

                if len(waypoints) < 2:
                    print(f"⚠️  sin coords en DB (score={score:.2f})")
                    sin_geometria.append({
                        "ruta":    ruta,
                        "tramo":   nombre_sc,
                        "estado":  estado,
                        "match_db": t_db.get("TramoNormalizado", ""),
                        "score":   round(score, 3),
                        "motivo":  "coordenadas_vacias_en_db",
                    })
                    continue

                # Obtener geometria
                if args.sin_osrm:
                    geometria  = waypoints
                    fuente_geo = "waypoints"
                else:
                    time.sleep(PAUSA_OSRM)
                    geometria = osrm_ruta(waypoints)
                    if geometria:
                        fuente_geo = "osrm"
                        print(f"OSRM ({len(geometria)} pts)", end=" ")
                    else:
                        geometria  = waypoints
                        fuente_geo = "waypoints_fallback"

                print(f"-> {estado} (score={score:.2f})")

                resultado.append({
                    "ruta":        ruta,
                    "tramo":       nombre_sc,
                    "tramo_db":    t_db.get("TramoNormalizado", ""),
                    "provincia":   t_db.get("NombreProvincia", ""),
                    "calzada":     calzada,
                    "estado":      estado,
                    "estado_raw":  estado_raw,
                    "obs":         obs,
                    "actualizado": actualizado,
                    "orden":       t_db.get("Orden", 0),
                    "geo":         geometria,
                    "fuente_geo":  fuente_geo,
                    "match_score": round(score, 3),
                })
            else:
                print(f"⚠️  sin match en DB")
                sin_geometria.append({
                    "ruta":   ruta,
                    "tramo":  nombre_sc,
                    "estado": estado,
                    "motivo": "sin_match_en_db",
                })

    # Enriquecer provincia desde DB
    provincias_db = {}
    for t in db_data:
        ruta = RUTAS.get(t.get("IdRuta"), str(t.get("IdRuta")))
        nombre = t.get("TramoNormalizado", "")
        prov_id = t.get("IdProvincia", 0)
        prov_map = {
            1:"Bahia Blanca", 2:"Buenos Aires", 3:"Catamarca", 4:"Chaco",
            5:"Chubut", 6:"Cordoba", 7:"Corrientes", 8:"Entre Rios",
            9:"Formosa", 10:"Jujuy", 11:"La Pampa", 12:"La Rioja",
            13:"Mendoza", 14:"Misiones", 15:"Neuquen", 16:"Rio Negro",
            17:"Salta", 18:"San Juan", 19:"San Luis", 20:"Santa Cruz",
            21:"Santa Fe", 22:"Santiago del Estero", 23:"Tierra del Fuego",
            24:"Tucuman",
        }
        provincias_db[f"{ruta}|{nombre}"] = prov_map.get(prov_id, "")

    for r in resultado:
        key = f"{r['ruta']}|{r['tramo_db']}"
        if not r["provincia"] and key in provincias_db:
            r["provincia"] = provincias_db[key]

    # Estadisticas
    cnt = {"transitable":0, "precaucion":0, "cortada":0, "default":0}
    for t in resultado:
        cnt[t["estado"]] = cnt.get(t["estado"], 0) + 1

    output = {
        "metadata": {
            "fecha_generacion":  datetime.now().isoformat(),
            "total_tramos":      len(resultado),
            "sin_geometria":     len(sin_geometria),
            "transitables":      cnt["transitable"],
            "precaucion":        cnt["precaucion"],
            "cortadas":          cnt["cortada"],
            "sin_estado":        cnt["default"],
        },
        "tramos": resultado,
    }

    Path(args.output).write_text(
        json.dumps(output, ensure_ascii=False, indent=2), encoding="utf-8"
    )

    # Log de tramos sin geometria
    Path(args.log).write_text(
        json.dumps(sin_geometria, ensure_ascii=False, indent=2), encoding="utf-8"
    )

    print(f"\n{'─'*55}")
    print(f"  Total tramos con geometria:  {len(resultado)}")
    print(f"  Sin geometria (ver log):     {len(sin_geometria)}")
    print(f"  Transitables:                {cnt['transitable']}")
    print(f"  Precaucion:                  {cnt['precaucion']}")
    print(f"  Cortadas:                    {cnt['cortada']}")
    print(f"  Sin estado:                  {cnt['default']}")
    print(f"  Archivo principal:           {args.output}")
    print(f"  Log sin geometria:           {args.log}")
    print(f"{'─'*55}")
    print(f"\n  Los tramos en {args.log} necesitan ser insertados")
    print(f"  en la DB con sus coordenadas/waypoints.")

if __name__ == "__main__":
    main()