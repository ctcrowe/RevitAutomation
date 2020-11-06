using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CC_Library
{
    public static class Dictionary_Master
    {
        private static string Foldername = "DataFiles".GetMyDocs();
        private static string Filename = Foldername + "\\Master_Dictionary.csv";

        public static void AddToDictionary(this string phrase)
        {
            if (!Directory.Exists(Foldername))
                Directory.CreateDirectory(Foldername);
            List<string> Lines = new List<string> { phrase };
            File.AppendAllLines(Filename, Lines);
        }
    }
}
