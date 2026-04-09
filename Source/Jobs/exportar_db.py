"""
exportar_db.py
==============
Exporta la tabla Tramo a tramos_db.json conectándose directo a SQL Server.
Incluye JOINs con Ruta, Provincia y Calzada para traer los nombres resueltos.

Requisitos:
    pip install pyodbc

Uso:
    python exportar_db.py
    python exportar_db.py --server . --db VialidadRutas
"""

import json
import argparse
import pyodbc
from pathlib import Path

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--server",  default="localhost")
    parser.add_argument("--db",      default="Rutas")
    parser.add_argument("--output",  default="tramos_db.json")
    parser.add_argument("--user",    default="")
    parser.add_argument("--password",default="")
    args = parser.parse_args()

    if args.user:
        conn_str = (
            f"DRIVER={{ODBC Driver 17 for SQL Server}};"
            f"SERVER={args.server};DATABASE={args.db};"
            f"UID={args.user};PWD={args.password};"
        )
    else:
        conn_str = (
            f"DRIVER={{ODBC Driver 17 for SQL Server}};"
            f"SERVER={args.server};DATABASE={args.db};"
            f"Trusted_Connection=yes;"
        )

    print(f"\nConectando a {args.server}/{args.db}...")
    try:
        conn = pyodbc.connect(conn_str)
    except pyodbc.Error:
        try:
            conn_str = conn_str.replace("ODBC Driver 17 for SQL Server", "SQL Server")
            conn = pyodbc.connect(conn_str)
            print("  Conectado con driver legacy")
        except pyodbc.Error as e:
            print(f"  Error: {e}")
            return

    query = """
        SELECT
            t.IdTramo,
            t.IdProvincia,
            p.Nombre        AS NombreProvincia,
            t.IdRuta,
            r.Nombre        AS NombreRuta,
            t.IdCalzada,
            c.Nombre        AS NombreCalzada,
            t.TramoNormalizado,
            t.TramoDesnormalizado,
            t.Coordenadas,
            t.Detalle,
            t.Observaciones,
            t.Orden
        FROM Tramo t
        INNER JOIN Provincia p ON p.IdProvincia = t.IdProvincia
        INNER JOIN Ruta      r ON r.IdRuta      = t.IdRuta
        INNER JOIN Calzada   c ON c.IdCalzada   = t.IdCalzada
        ORDER BY t.IdRuta, t.IdProvincia, t.Orden
    """

    print("  Ejecutando query...")
    cursor = conn.cursor()
    cursor.execute(query)
    cols = [c[0] for c in cursor.description]
    rows = cursor.fetchall()
    conn.close()

    print(f"  Filas obtenidas: {len(rows)}")

    resultado = []
    for row in rows:
        d = {}
        for i, col in enumerate(cols):
            val = row[i]
            if isinstance(val, (bytes, bytearray)):
                val = val.decode("utf-8", errors="replace")
            d[col] = val
        resultado.append(d)

    Path(args.output).write_text(
        json.dumps(resultado, ensure_ascii=False, indent=2),
        encoding="utf-8"
    )

    print(f"  Archivo guardado: {args.output}")
    print(f"  Campos exportados: {cols}")
    print(f"  Listo.")

if __name__ == "__main__":
    main()