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
        private static string Dataset = directory + "\\CC_XMLData";
        private static string InputFile = directory + "\\CC_MFData.xml";
        private static string OutputFile = directory + "\\CC_MasterformatPredictor.xml";

        internal static string RunFormula(List<PredictionElement> PEs)
        {
            List<Prediction> data = new List<Prediction>();
            foreach(var p in PEs)
            {
                foreach(var d in p.Options)
                {
                    if(!data.Any(x => x.Name == d.Name))
                    {
                        data.Add(new Prediction(d));
                    }
                    else
                    {
                        data.Where(x => x.Name == d.Name).First().Combine(d);
                    }
                }
            }
            return data[data.IndexOf(data.Where(x => x.Value == data.Max(y => y.Value)).First())].Name;
        }
        public static void Run()
        {
            if(File.Exists(InputFile))
            {
                int count = 0;
                int correct = 0;
                double accuracy = 0;
                XDocument indoc = XDocument.Load(InputFile);
                List<PredictionPhrase> phrases = PredictionPhrase.GetData();
                List<PredictionElement> elements = new List<PredictionElement>();

                foreach(XElement ele in indoc.Root.Elements())
                {
                    elements.Add(new PredictionElement(ele));
                }
                while(true)
                {
                    foreach(var v in elements)
                    {
                        foreach(
                    }
                }
            }
        }
    }
}
