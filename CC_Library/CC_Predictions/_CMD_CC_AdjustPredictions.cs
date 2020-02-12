using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

namespace CC_Library
{
    public class AdjustPredictions
    {
        // Formula => x = SUM
        // if (Positive > Negative) => Weight * (((Positive - Negative) ^ 2) / (Count ^ 2))
        // if (Negative > Positive) => -Weight * (((Positive - Negative) ^ 2) / (Count ^ 2))
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string InputFile = directory + "\\CC_MFData.xml";
        private static string OutputFile = directory + "\\CC_MasterformatPredictor.xml";

        public static List<Prediction> RunFormula(List<PredictionElement> PEs)
        {
            List<Prediction> data = new List<Prediction>();
            foreach(var p in PEs)
            {
                foreach(var d in p.Options)
                {
                    if(!data.Any(x => x.Name == d.Name))
                    {
                    }
                }
            }
        }
        public static void run()
        {
            if(File.Exists(InputFile))
            {
                int count = 0;
                int correct = 0;
                double accuracy = 0;
                XDocument indoc = XDocument.Load(InputFile);
                
            }
        }
    }
}
