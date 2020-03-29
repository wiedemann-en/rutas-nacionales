using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vialidad.Services.Normalizer
{
    public static class NormalizerKey
    {
        private static Dictionary<string, string> _replaces;

        static NormalizerKey()
        {
            Initialize();
        }

        public static string Normalize(string info)
        {
            //Pasamos todo a mayúsculas
            info = info.ToLower().Trim();

            //Eliminamos espacios adicionales
            var trimmer = new Regex(@"\s\s+");
            info = trimmer.Replace(info, " ");
            info = info.Trim();

            //Normalizamos palabras comunes
            foreach (var item in _replaces)
                info = info.Replace(item.Key, item.Value);
            foreach (var item in _replaces)
                info = info.Replace(item.Key, item.Value);

            return info;
        }

        private static void Initialize()
        {
            _replaces = new Dictionary<string, string>();
            _replaces.Add("á", "a");
            _replaces.Add("é", "e");
            _replaces.Add("í", "i");
            _replaces.Add("ó", "o");
            _replaces.Add("ú", "u");
            _replaces.Add("ñ", "n");
            _replaces.Add("  ", " ");
            _replaces.Add(" - ", "-");
            _replaces.Add(" / ", " ");
            _replaces.Add("/", "");
            _replaces.Add(" ", "-");
            _replaces.Add("a-0", "a0");
        }
    }
}
