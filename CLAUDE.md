# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Interactive map of Argentine national road status, sourced from Vialidad Nacional (argentina.gob.ar). The current version is a static site + Python pipeline. There is also a legacy .NET Web API backend in `Source/Back/` (Visual Studio solution `Vialidad.sln`) that is no longer the active approach.

## Running the Frontend

The frontend has no build step — it is a single HTML file that loads `json/tramos.json` via `$.getJSON`. To serve it locally:

```bash
cd Source/Front
python -m http.server 3000
# then open http://localhost:3000
```

If `tramos.json` is missing, the map shows an error with instructions to regenerate it.

## Python Pipeline (Source/Jobs/)

All scripts run from `Source/Jobs/` with Python 3.11+.

### Install dependencies

```bash
pip install playwright beautifulsoup4 requests
playwright install chromium
# for DB export only:
pip install pyodbc
```

### Key scripts and their roles

| Script | Purpose |
|---|---|
| `scraper_rutas.py` | Scrapes argentina.gob.ar road status table via Playwright → `rutas_estado.json` |
| `generar_tramos.py` | Merges scraper output with geometry DB → `Source/Front/json/tramos.json` |
| `exportar_db.py` | Exports SQL Server geometry DB → `Source/Front/json/tramos_db.json` (run when DB changes) |
| `diagnostico_match.py` | Shows tramos in `tramos.json` with `estado=default` for debugging match failures |
| `tramos_sin_coordenadas.py` | Shows scraper tramos that have no matching geometry in the DB |

### Common workflow

```bash
# Step 1 – scrape current road status
python scraper_rutas.py --output rutas_estado.json

# Step 2 – generate tramos.json (fast, no OSRM)
python generar_tramos.py \
  --db ../Front/json/tramos_db.json \
  --estado rutas_estado.json \
  --output ../Front/json/tramos.json \
  --sin-osrm

# Step 2 alt – with OSRM routing (smoother polylines, slower, requires internet)
python generar_tramos.py \
  --db ../Front/json/tramos_db.json \
  --estado rutas_estado.json \
  --output ../Front/json/tramos.json
```

### Updating the geometry DB

When new road sections need to be added, export from SQL Server and commit the result:

```bash
python exportar_db.py --server localhost --db Rutas
# copies output to Source/Front/json/tramos_db.json by default
```

## Data Flow

```
argentina.gob.ar
       ↓  scraper_rutas.py
rutas_estado.json           tramos_db.json  ← SQL Server (exportar_db.py)
       └──────────┬──────────────┘
              generar_tramos.py
                   ↓
         Source/Front/json/tramos.json
                   ↓
         Source/Front/index.html  (Leaflet map)
```

## Automated CI (GitHub Actions)

`.github/workflows/actualizar_rutas.yml` runs daily at 06:00 Argentina time (09:00 UTC) and on manual dispatch. It:
1. Runs `scraper_rutas.py` → `rutas_estado.json`
2. Runs `generar_tramos.py --sin-osrm` → `tramos.json`
3. Commits and pushes both files if they changed

The geometry DB (`tramos_db.json`) is **not** updated by CI — it must be updated manually via `exportar_db.py` when road sections change.

## Frontend Architecture (`Source/Front/index.html`)

Single-file SPA — all CSS, JS, and HTML in one file. No framework, no build step.

- **Map**: Leaflet 1.9.4 with a MapTiler/MapboxGL vector tile base layer (key hardcoded as `dLbzt96PKK6rA56B4jaU`, style ID `e460f1aa-...`)
- **Data**: Loads `json/tramos.json` on page load, renders each tramo as a `L.polyline`
- **Filters**: Estado (transitable/precaución/cortada/default), Calzada (pavimento/ripio/tierra), Provincia multiselect, Ruta multiselect, free-text search
- **Colors**: `transitable=#22c55e`, `precaucion=#f59e0b`, `cortada=#ef4444`, `default=#3b82f6`
- **Line style**: Ripio = dashed `8 5`, Tierra = dotted `3 5`, Pavimento = solid
- Tramos are rendered in batches of 50 via `setTimeout` to keep the UI responsive

## Data Formats

### `tramos.json` (frontend input)
```json
{
  "metadata": { "fecha_generacion": "...", "total_tramos": N, ... },
  "tramos": [
    {
      "ruta": "3", "tramo": "Bahía Blanca - Neuquén",
      "provincia": "Buenos Aires", "calzada": "PAVIMENTO",
      "estado": "transitable", "estado_raw": "Habilitada",
      "obs": "", "actualizado": "...",
      "geo": [[-lat, lng], ...],
      "fuente_geo": "waypoints|osrm|waypoints_fallback",
      "match_score": 0.85
    }
  ]
}
```

### `tramos_db.json` (geometry DB)
Array of rows from the SQL Server `Tramo` table joined with `Ruta`, `Provincia`, `Calzada`. Key fields: `IdRuta`, `NombreRuta`, `TramoNormalizado`, `TramoDesnormalizado`, `Coordenadas` (lat,lng pairs separated by `/`), `Orden`.

### Tramo matching logic (`generar_tramos.py`)
1. Exact match on `TramoDesnormalizado` (case/accent insensitive)
2. Fuzzy Jaccard score on `TramoNormalizado` (threshold 0.15), with bonuses for long words (localities) and shared segment endpoints
3. `RUTA_ALIAS` dict maps scraper route names to DB names (e.g., `"A-001" → "A001"`)

## Legacy Backend (`Source/Back/`)

A .NET Framework 4.6 Web API solution (`Vialidad.sln`) with projects: `Vialidad` (scraper), `Vialidad.Model`, `Vialidad.Services`, `Vialidad.Contracts`, `Vialidad.Routing`, `Vialidad.Cache`, `Vialidad.Logger`, `Vialidad.Scheduler`, `Vialidad.Utils`, `Vialidad.Web.Api`. This is no longer the active data source — the Python pipeline has replaced it.
