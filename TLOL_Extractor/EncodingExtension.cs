using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLOL_Extractor
{
    public static class EncodingExtension
    {
        public static Encoding GetUTF8WithoutBOM()
        {
            Encoding utf8WithoutBom = new UTF8Encoding(false);
            return utf8WithoutBom;
        }
    }
}
