using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CC_Library
{
    public static class Masterformat_Master
    {
        private static string Foldername = "DataFiles".GetMyDocs();
        private static string Filename = Foldername + "\\Master_Masterformat.csv";

        public static void AddToMF(this string phrase, int Numb)
        {
            if (!Directory.Exists(Foldername))
                Directory.CreateDirectory(Foldername);
            List<string> Lines = new List<string> { phrase + "," + Numb};
            File.AppendAllLines(Filename, Lines);
        }
    }
}
