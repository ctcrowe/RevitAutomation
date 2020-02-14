using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace CC_Library
{
    public class GenPredictions
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string FileName = directory + "\\CC_MFData.xml";
        
        public static string Run(string s)
        {
            PredictionPhrase p = new PredictionPhrase(s);
            return AdjustPredictions.RunFormula(p.Elements);
        }
    }
}
