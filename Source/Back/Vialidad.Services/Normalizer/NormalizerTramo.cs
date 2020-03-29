using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vialidad.Services.Normalizer
{
    public static class NormalizerTramo
    {
        private static Dictionary<string, string> _replaces;

        static NormalizerTramo()
        {
            Initialize();
        }

        public static string Normalize(string info)
        {
            //Pasamos todo a mayúsculas
            info = info.ToUpper().Trim();

            //Normalizamos palabras comunes
            foreach (var item in _replaces)
                info = info.Replace(item.Key, item.Value);

            //Otra vuelta más para que no queden errores
            foreach (var item in _replaces)
                info = info.Replace(item.Key, item.Value);
            foreach (var item in _replaces)
                info = info.Replace(item.Key, item.Value);

            info = info.TrimEnd('.');

            //Eliminamos espacios adicionales
            var trimmer = new Regex(@"\s\s+");
            info = trimmer.Replace(info, " ");
            info = info.Trim();

            return info;
        }

        private static void Initialize()
        {
            _replaces = new Dictionary<string, string>();
            _replaces.Add("Á", "A");
            _replaces.Add("É", "E");
            _replaces.Add("Í", "I");
            _replaces.Add("Ó", "O");
            _replaces.Add("Ú", "U");
            _replaces.Add("EMP.", "EMPALME ");
            _replaces.Add("EMP ", "EMPALME ");
            _replaces.Add("EMPALME. ", "EMPALME ");
            _replaces.Add("ACC.", "ACCESO ");
            _replaces.Add("SECC.", "SECCION ");
            _replaces.Add("CAMP.", "CAMPAMENTO ");
            _replaces.Add("LTE.", "LIMITE ");
            _replaces.Add("LIM. C/", "LIMITE CON ");
            _replaces.Add("LIM.", "LIMITE ");
            _replaces.Add("LT.", "LIMITE CON ");
            _replaces.Add("LMTE. C/", "LIMITE CON ");
            _replaces.Add("L/", "LIMITE CON ");
            _replaces.Add("LIM ", "LIMITE ");
            _replaces.Add("LTE ", "LIMITE ");
            _replaces.Add("LIMITE C.", "LIMITE CON ");
            _replaces.Add("R. PROVINCIAL", "RUTA PROVINCIAL ");
            _replaces.Add("RUTA P.", "RUTA PROVINCIAL ");
            _replaces.Add("R PROV.", "RUTA PROVINCIAL ");
            _replaces.Add("R. P.", "RUTA PROVINCIAL ");
            _replaces.Add("R.P.", "RUTA PROVINCIAL ");
            _replaces.Add("R.P", "RUTA PROVINCIAL ");
            _replaces.Add("RP.", "RUTA PROVINCIAL ");
            _replaces.Add("RP ", "RUTA PROVINCIAL ");
            //_replaces.Add("R.", "RUTA ");
            _replaces.Add("PROV.", "PROVINCIAL ");
            _replaces.Add("PROV ", "PROVINCIAL ");
            _replaces.Add("PCIAL", "PROVINCIAL ");
            _replaces.Add("PCIA.", "PROVINCIA ");
            _replaces.Add("R. NACIONAL", "RUTA NACIONAL ");
            _replaces.Add("RUTA N.", "RUTA NACIONAL ");
            _replaces.Add("R NAC.", "RUTA NACIONAL ");
            _replaces.Add("R. N.", "RUTA NACIONAL ");
            _replaces.Add("R.N.", "RUTA NACIONAL ");
            _replaces.Add("R.N", "RUTA NACIONAL ");
            _replaces.Add("RN.", "RUTA NACIONAL ");
            _replaces.Add("RN ", "RUTA NACIONAL ");
            _replaces.Add("RNNº", "RUTA NACIONAL Nº");
            _replaces.Add("RN9N", "RUTA NACIONAL Nº9N");
            _replaces.Add("EX R 9", "RUTA NACIONAL 9");
            _replaces.Add("NAC.", "NACIONAL ");
            _replaces.Add("NAC ", "NACIONAL ");
            _replaces.Add("PTE ", "PUENTE ");
            _replaces.Add("PTE. ", "PUENTE ");
            _replaces.Add("KM.", "KM ");
            _replaces.Add("ANT.", "ANTIGUA");
            //_replaces.Add("B.", "BARRIO ");
            //_replaces.Add("P.", "PUNTA ");
            _replaces.Add("(AUTOPISTA)", "");
            _replaces.Add("INF.", "INFIERNO ");
            _replaces.Add("AV.", "AVENIDA ");
            _replaces.Add("AVDA.", "AVENIDA ");
            _replaces.Add("ING.", "INGENIERO ");
            _replaces.Add("TTE.", "TENIENTE ");
            _replaces.Add("GRAL.", "GENERAL ");
            _replaces.Add("CNEL.", "CORONEL ");
            _replaces.Add("GOB.", "GOBERNADOR ");
            _replaces.Add("GDOR.", "GOBERNADOR ");
            _replaces.Add("REP.", "REPUBLICA ");
            _replaces.Add("INDEP.", "INDEPENDENCIA ");
            _replaces.Add("INTER.", "INTERNACIONAL ");
            _replaces.Add("INTERNAD.", "INTERNACIONAL ");
            _replaces.Add("CIRCUNV.", "CIRCUNVALACION ");
            _replaces.Add("AEROP.", "AEROPUERTO ");
            _replaces.Add("VA.", "VILLA ");
            _replaces.Add("V. M. ", "VILLA MARIANO ");
            _replaces.Add("V.M. ", "VILLA MARIANO ");
            _replaces.Add("SAN FCO.", "SAN FRANCISCO ");
            _replaces.Add("S.A. OESTE", "SAN ANTONIO OESTE ");
            _replaces.Add("S.A.OESTE", "SAN ANTONIO OESTE ");
            _replaces.Add("S.A. ESTE", "SAN ANTONIO ESTE ");
            _replaces.Add("S.A.ESTE", "SAN ANTONIO ESTE ");
            _replaces.Add("S.C.", "SAN CARLOS ");
            _replaces.Add("CRO. RIVADAVIA", "COMODORO RIVADAVIA ");
            _replaces.Add("CRO.RIVADAVIA", "COMODORO RIVADAVIA ");
            _replaces.Add("C.RIVADAVIA", "COMODORO RIVADAVIA ");
            _replaces.Add("T. LAUQUEN", "TRENQUE LAUQUEN ");
            _replaces.Add("PTO.MADRYN", "PUERTO MADRYN ");
            _replaces.Add("S.M.", "SAN MIGUEL ");
            _replaces.Add("S.M:", "SAN MIGUEL ");
            _replaces.Add("S. M.", "SAN MIGUEL ");
            _replaces.Add("BS. AS.", "BUENOS AIRES ");
            _replaces.Add("STA. RSA.", "SANTA ROSA ");
            _replaces.Add("STA. RSA", "SANTA ROSA ");
            _replaces.Add("STO TOME", "SANTO TOME ");
            _replaces.Add("BS.AS.", "BUENOS AIRES ");
            _replaces.Add("STA.CRUZ", "SANTA CRUZ ");
            _replaces.Add("L. PAMPA", "LA PAMPA ");
            _replaces.Add("STIAGO.", "SANTIAGO ");
            _replaces.Add("SGO.", "SANTIAGO ");
            _replaces.Add("SANTIAGO / ", "SANTIAGO DEL ESTERO / ");
            _replaces.Add("CBA.", "CORDOBA ");
            _replaces.Add("S.S.", "SAN SALVADOR ");
            _replaces.Add("SFE ", "SANTA FE ");
            _replaces.Add("S.FE", "SANTA FE ");
            _replaces.Add("S. FE", "SANTA FE ");
            _replaces.Add("SANTA FE - ER", "SANTA FE - ENTRE RIOS");
            _replaces.Add("SANTE FE - ER", "SANTA FE - ENTRE RIOS");
            _replaces.Add("SFE - ER", "SANTA FE - ENTRE RIOS");
            _replaces.Add("SFE-ER", "SANTA FE - ENTRE RIOS");
            _replaces.Add("SGO / TUC", "SANTIAGO DEL ESTERO / TUCUMAN");
            _replaces.Add("C. SALTA", "CON SALTA ");
            _replaces.Add("C/", "CON ");
            _replaces.Add("STA. ", "SANTA ");
            _replaces.Add("STA .", "SANTA ");
            _replaces.Add("S / N", "SIN NUMERO ");
            _replaces.Add("Aº", "ARROYO ");
            _replaces.Add("A°", "ARROYO ");
            _replaces.Add("Rº", "RIO ");
            _replaces.Add("R°", "RIO ");
            _replaces.Add("N°", "Nº");
            _replaces.Add("Nº", "Nº ");
            _replaces.Add("( ", "(");
            _replaces.Add(" )", ")");
            _replaces.Add("(", " (");
            _replaces.Add(")", ") ");
            _replaces.Add("-", " - ");
            _replaces.Add("//", " / ");
            _replaces.Add("/", " / ");
            _replaces.Add("&QUOT;", "\"");
            _replaces.Add(".  - ", " ");
            _replaces.Add(". -", " ");
            _replaces.Add(".-", " ");
            _replaces.Add("A-001", "A001");
            _replaces.Add("A - 001", "A001");
            _replaces.Add("A - 012", "A012");
            _replaces.Add("A - 008", "A008");
        }
    }
}
