using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

namespace CC_Library
{
    public delegate void Write(string s);
    public static class AdjustPredictions
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CC_XMLData";
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
                        data.Add(new Prediction(d, p));
                    }
                    else
                    {
                        data.Where(x => x.Name == d.Name).First().Combine(p);
                    }
                }
            }
            if (!data.Any())
                return "No Prediction Found";
            return data[data.IndexOf(data.Where(x => x.Value == data.Max(y => y.Value)).First())].Name;
        }
        internal static PredictionElement GetElement(PredictionElement ele, List<PredictionElement> PredList, int digit)
        {
            double x = Math.Pow(10, digit * -1);
            double[] y = new double[10];
            for(int i = 0; i < 10; i++)
            {
                PredictionElement e = ele;
                e.Weight += i * x;
            }
            return ele;
        }
        internal static void RunAdjustment(this List<PredictionElement> elements, Write w)
        {
            /*
            int count = 0;
            int cor = 0;
            int cor2 = 0;
            int cor3 = 0;*/
                
            List<PredictionPhrase> phrases = PredictionPhrase.GetData();
            
            while(true)
            {
                foreach(var e in elements)
                {
                    e.Weight = 0;
                    for(int i = 1; i <= 3; i++)
                    {
                        elements.AdjustWeight(e.Word, i);
                    }
                    w(e.Word + " : " + e.Weight.ToString() + " , " + e.Accuracy.ToString());
                }
                XDocument output = new XDocument(new XElement("MASTERFORMAT")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                foreach (var element in elements)
                {
                    XElement e = element.CreateXML();
                    output.Root.Add(e);
                }
                output.Save(OutputFile);
            }
        }
    }
}
