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
        private static string Input = directory + "\\CC_MasterformatPredictor.xml";
        
        public static string Run(string s)
        {
            List<string> x = s.SplitTitle();
            List<PredictionElement> pes = new List<PredictionElement>();
            XDocument doc = XDocument.Load(Input);
            foreach(string y in x)
            {
                if(doc.Root.Elements().Any(x => x.Attribute("WORD").Value == y))
                {
                    pes.Add(new PredictionElement(doc.Root.Elements().Where(x => x.Attribute("WORD").Value == y).First());
                }
            }
            return AdjustPredictions.RunFormula(pes);
        }
    }
}
