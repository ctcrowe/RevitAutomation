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
                        data.Add(new Prediction(d, p));
                    }
                    else
                    {
                        data.Where(x => x.Name == d.Name).First().Combine(p);
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
                int cor = 0;
                int cor2 = 0;
                int cor3 = 0;
                double Accuracy = 0;
                XDocument indoc = XDocument.Load(InputFile);
                List<PredictionPhrase> phrases = PredictionPhrase.GetData();
                List<PredictionElement> elements = new List<PredictionElement>();

                foreach(XElement ele in indoc.Root.Elements())
                {
                    elements.Add(new PredictionElement(ele));
                }
                while(true)
                {
                    double Accuracy = cor / count;
                    cor = 0;
                    cor2 = 0;
                    cor3 = 0;
                    count = 0;
                    foreach(var v in elements)
                    {
                        double MaxChange = Math.Abs(1 - v.Weight) * Math.Abs(1 - Accuracy);
                        var vneg = v;
                        vneg.Weight-= MaxChange;
                        var vpos = v;
                        vpos.Weight += MaxChange
                        
                        var eleNeg = elements;
                        var elePos = elements;
                        
                        eleNeg.RemoveAt(eleNeg.IndexOf(eleNeg.Where(x => x.Word == v.Word).First()));
                        eleNeg.Add(vneg);
                        elePos.RemoveAt(eleNeg.IndexOf(eleNeg.Where(x => x.Word == v.Word).First()));
                        elePos.Add(vneg);
                        
                        foreach(var p in phrases.Where(x => x.Phrase.Contains(v.Word)))
                        {
                            var PhraseElements = elements.Where(x => p.Elements.Any(y => y == x.Word));
                            var PENeg = elements.Where(x => p.Elements.Any(y => y == x.Word));
                            var PEPos = elements.Where(x => p.Elements.Any(y => y == x.Word));
                            PENeg.Add(vneg);
                            PEPos.Add(vpos);
                            count++;
                            if(RunFormula(PhraseElements) == p.Prediction)
                                cor++;
                            if(RunFormula(PENeg) == p.Prediction)
                                cor2++;
                            if(RunFormula(PEPos) == p.Prediction)
                                cor3++;
                        }
                    }
                }
            }
        }
    }
}
