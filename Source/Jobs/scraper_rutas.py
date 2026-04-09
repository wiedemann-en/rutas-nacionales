"""
scraper_rutas.py  v3
====================
Scraper del Estado de Rutas Nacionales – argentina.gob.ar

Requisitos:
    pip install playwright beautifulsoup4
    playwright install chromium

Uso:
    python scraper_rutas.py
    python scraper_rutas.py --headful
    python scraper_rutas.py --output mi.json
"""

import json
import argparse
import time
from datetime import datetime
from pathlib import Path

try:
    from playwright.sync_api import sync_playwright, TimeoutError as PWTimeout
except ImportError:
    print("Falta playwright:  pip install playwright && playwright install chromium")
    raise

try:
    from bs4 import BeautifulSoup
except ImportError:
    print("Falta beautifulsoup4:  pip install beautifulsoup4")
    raise

URL       = "https://www.argentina.gob.ar/transporte/vialidad-nacional/estado-de-rutas"
TABLE_ID  = "ponchoTable"
TIMEOUT_MS = 30_000

CLASE_ESTADO = {
    "bg-verdin":    "transitable",
    "bg-mandarina": "precaucion",
    "bg-rojo":      "cortada",
    "bg-danger":    "cortada",
    "bg-warning":   "precaucion",
    "bg-success":   "transitable",
}

TEXTO_ESTADO = {
    "habilitada":     "transitable",
    "transitable":    "transitable",
    "normal":         "transitable",
    "corte parcial":  "precaucion",
    "precaucion":     "precaucion",
    "con precaucion": "precaucion",
    "corte total":    "cortada",
    "cortada":        "cortada",
    "inhabilitada":   "cortada",
    "cerrada":        "cortada",
    "intransitable":  "cortada",
}

def normalizar_estado(celda_td):
    span = celda_td.find("span", class_=True)
    if span:
        for cls in span.get("class", []):
            if cls in CLASE_ESTADO:
                return CLASE_ESTADO[cls], span.get_text(strip=True)
    texto = celda_td.get_text(strip=True).lower()
    for k, v in TEXTO_ESTADO.items():
        if k in texto:
            return v, celda_td.get_text(strip=True)
    return "default", celda_td.get_text(strip=True)

def parsear_pagina(html):
    soup = BeautifulSoup(html, "html.parser")
    tabla = soup.find("table", id=TABLE_ID)
    if not tabla:
        return []
    filas = []
    for tr in tabla.select("tbody tr"):
        celdas = tr.find_all("td")
        if len(celdas) < 5:
            continue
        def txt(i):
            return celdas[i].get_text(" ", strip=True) if i < len(celdas) else ""
        estado_norm, estado_raw = normalizar_estado(celdas[3])
        iconos = []
        if len(celdas) > 6:
            for img in celdas[6].find_all("img"):
                alt = img.get("alt", "").strip()
                if alt:
                    iconos.append(alt)
        filas.append({
            "provincia":   txt(0),
            "ruta":        txt(1).strip().lstrip("0") or txt(1),
            "tramo":       txt(2),
            "estado":      estado_norm,
            "estado_raw":  estado_raw,
            "calzada":     txt(4),
            "km":          txt(5),
            "iconos":      iconos,
            "obs":         txt(7) if len(celdas) > 7 else "",
            "actualizado": txt(8) if len(celdas) > 8 else "",
        })
    return filas

def scrape(headless=True):
    print(f"\nIniciando scraper v3 — {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print(f"URL: {URL}\n")

    todas_filas = []

    with sync_playwright() as p:
        browser = p.chromium.launch(
            headless=headless,
            args=["--no-sandbox", "--disable-setuid-sandbox"]
        )
        context = browser.new_context(
            viewport={"width": 1280, "height": 900},
            user_agent=(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
                "AppleWebKit/537.36 (KHTML, like Gecko) "
                "Chrome/120.0.0.0 Safari/537.36"
            ),
            locale="es-AR",
        )
        page = context.new_page()
        page.route("**/*.{png,jpg,jpeg,gif,woff,woff2,ttf}", lambda r: r.abort())

        print("Cargando pagina...")
        page.goto(URL, wait_until="domcontentloaded", timeout=TIMEOUT_MS)

        print("Esperando tabla #ponchoTable...")
        try:
            page.wait_for_selector(f"#{TABLE_ID} tbody tr", timeout=TIMEOUT_MS)
        except PWTimeout:
            print("ERROR: La tabla no aparecio.")
            page.screenshot(path="debug_screenshot.png", full_page=True)
            browser.close()
            return []

        # --- DIAGNOSTICO: ver que globals JS estan disponibles ---
        diag_fn = (
            "() => {"
            + "var keys = Object.keys(window).filter(function(k) {"
            + "  return k.toLowerCase().indexOf('datatable') >= 0"
            + "    || k.toLowerCase().indexOf('jquery') >= 0"
            + "    || k === '$' || k === 'jQuery' || k === 'poncho';"
            + "});"
            + "return {globals: keys};"
            + "}"
        )
        debug_js = page.evaluate(diag_fn)
        print(f"\nDEBUG JS globals: {debug_js}\n")
        # --- FIN DIAGNOSTICO ---

        # Detectar total de paginas
        paginas_totales = 1
        try:
            paginador = page.query_selector(f"#{TABLE_ID}_paginate")
            if paginador:
                nums = paginador.query_selector_all(
                    ".paginate_button:not(#ponchoTable_previous)"
                    ":not(#ponchoTable_next):not(#ponchoTable_ellipsis)"
                )
                paginas_totales = max(
                    int(n.inner_text().strip())
                    for n in nums if n.inner_text().strip().isdigit()
                )
        except Exception:
            pass

        print(f"Paginas detectadas: {paginas_totales}\n")

        for pagina_idx in range(paginas_totales):
            print(f"Pagina {pagina_idx+1}/{paginas_totales}...", end=" ", flush=True)

            if pagina_idx > 0:
                # Intentar via JS, con fallback a click en numero de pagina
                avanzado = False

                # Intento 1: jQuery global
                try:
                    page.evaluate(
                        f"jQuery('#{TABLE_ID}').DataTable().page({pagina_idx}).draw('page')"
                    )
                    avanzado = True
                    time.sleep(1.5)  # espera fija tras jQuery — más confiable que wait_for_function
                except Exception:
                    pass

                # Intento 2: window.jQuery
                if not avanzado:
                    try:
                        page.evaluate(
                            f"window.jQuery('#{TABLE_ID}').DataTable().page({pagina_idx}).draw('page')"
                        )
                        avanzado = True
                    except Exception:
                        pass

                # Intento 3: click en el numero de pagina del paginador
                if not avanzado:
                    try:
                        num_str = str(pagina_idx + 1)
                        clicked = page.evaluate(
                            f"(function() {{"
                            f"  var btns = document.querySelectorAll('#{TABLE_ID}_paginate .paginate_button a');"
                            f"  for (var i=0; i<btns.length; i++) {{"
                            f"    if (btns[i].innerText.trim() === '{num_str}') {{"
                            f"      btns[i].click(); return true;"
                            f"    }}"
                            f"  }}"
                            f"  return false;"
                            f"}})()"
                        )
                        if clicked:
                            avanzado = True
                    except Exception:
                        pass

                # Intento 4: click en boton siguiente
                if not avanzado:
                    try:
                        page.click(f"#{TABLE_ID}_next")
                        avanzado = True
                    except Exception:
                        pass

                if not avanzado:
                    print(f"No se pudo avanzar a pagina {pagina_idx+1}. Deteniendo.")
                    break

                # Esperar que el info cambie — buscar la fila esperada de forma flexible
                expected_from = pagina_idx * 15 + 1
                try:
                    page.wait_for_function(
                        f"(function() {{"
                        f"  var rows = document.querySelectorAll('#{TABLE_ID} tbody tr');"
                        f"  if (rows.length === 0) return false;"
                        f"  var id = rows[0].getAttribute('id');"
                        f"  return id && id === 'id_{pagina_idx * 15}';"
                        f"}})()",
                        timeout=8000
                    )
                except PWTimeout:
                    time.sleep(1)  # si igual falla, pausa mínima y seguimos

                time.sleep(0.8)

            html = page.content()
            filas = parsear_pagina(html)
            todas_filas.extend(filas)
            print(f"{len(filas)} filas")

        browser.close()

    print(f"\nTotal filas extraidas: {len(todas_filas)}")
    return todas_filas

def agrupar_por_ruta(filas):
    agrupado = {}
    for f in filas:
        ruta = f["ruta"]
        if ruta not in agrupado:
            agrupado[ruta] = []
        agrupado[ruta].append({
            "provincia":   f["provincia"],
            "tramo":       f["tramo"],
            "estado":      f["estado"],
            "estado_raw":  f["estado_raw"],
            "calzada":     f["calzada"],
            "km":          f["km"],
            "obs":         f["obs"],
            "iconos":      f["iconos"],
            "actualizado": f["actualizado"],
        })
    return agrupado

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output",  default="rutas_estado.json")
    parser.add_argument("--headful", action="store_true")
    args = parser.parse_args()

    filas = scrape(headless=not args.headful)

    if not filas:
        print("No se extrajeron datos.")
        return

    por_ruta = agrupar_por_ruta(filas)
    conteos = {"transitable": 0, "precaucion": 0, "cortada": 0, "default": 0}
    for tramos in por_ruta.values():
        for t in tramos:
            conteos[t["estado"]] = conteos.get(t["estado"], 0) + 1

    output = {
        "metadata": {
            "fuente":       URL,
            "fecha_scrape": datetime.now().isoformat(),
            "total_rutas":  len(por_ruta),
            "total_tramos": len(filas),
            "transitables": conteos["transitable"],
            "precaucion":   conteos["precaucion"],
            "cortadas":     conteos["cortada"],
            "sin_estado":   conteos["default"],
        },
        "rutas": por_ruta,
    }

    Path(args.output).write_text(
        json.dumps(output, ensure_ascii=False, indent=2), encoding="utf-8"
    )

    print(f"\nTransitables:  {conteos['transitable']}")
    print(f"Precaucion:    {conteos['precaucion']}")
    print(f"Cortadas:      {conteos['cortada']}")
    print(f"Archivo:       {args.output}")
    print(f"\nCopia el objeto 'rutas' del JSON y pegalo en ESTADOS_SCRAPED del mapa.")

if __name__ == "__main__":
    main()