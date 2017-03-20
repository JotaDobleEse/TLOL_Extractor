using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLOL_Extractor
{
    public static class StringReplacements
    {
        public static string ReplaceSpanishChars(this string s)
        {
            string aux = s;
            aux = aux.Replace('ñ', 'n')
            .Replace('á', 'a')
            .Replace('é', 'e')
            .Replace('í', 'i')
            .Replace('ó', 'o')
            .Replace('ú', 'u')
            .Replace('Ñ', 'N')
            .Replace('Á', 'A')
            .Replace('É', 'E')
            .Replace('Í', 'I')
            .Replace('Ó', 'O')
            .Replace('Ú', 'U')
            .Replace('¿', ' ')
            .Replace('¡', ' ');
            return aux;
        }
    }
}
