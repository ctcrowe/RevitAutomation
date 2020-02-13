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
                
                XDocument indoc = XDocument.Load(InputFile);
                XDocument output = new XDocument(new XElement("MASTERFORMAT")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                
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
                        cor = 0;
                        cor2 = 0;
                        cor3 = 0;
                        count = 0;
                        double poscw = Math.Abs(1 - v.Weight);
                        double negcw = Math.Abs(0 - Math.Abs(v.Weight));
                        double mca = 1 - v.Accuracy;
                        double NegChange = negw * mca;
                        double PosChagne = poscw * mca;
                        
                        var eleNeg = elements;
                        var elePos = elements;
                        eleNeg.Where(x => x.Word == v.Word).First()).Weight -= NegChange;
                        elePos.Where(x => x.Word == v.Word).First()).Weight += PosChange;
                        
                        foreach(var p in phrases.Where(x => x.Phrase.Contains(v.Word)))
                        {
                            var PhraseElements = elements.Where(x => p.Elements.Any(y => y == x.Word));
                            var PENeg = eleNeg.Where(x => p.Elements.Any(y => y == x.Word));
                            var PEPos = elePos.Where(x => p.Elements.Any(y => y == x.Word));
                            count++;
                            if(RunFormula(PhraseElements) == p.Prediction)
                                cor++;
                            if(RunFormula(PENeg) == p.Prediction)
                                cor2++;
                            if(RunFormula(PEPos) == p.Prediction)
                                cor3++;
                        }
                        if(cor2 > cor3 && cor2 > cor)
                        {
                            elements.Where(x => x.Word == v.Word).First().Accuracy = cor2 / count;
                            elements.Where(x => x.Word == v.Word).First().Weight -= NegChange;
                        }
                        else
                        {
                            if(cor3 > cor2 && cor3 > cor)
                            {
                                elements.Where(x => x.Word == v.Word).First().Accuracy = cor3 / count;
                                elements.Where(x => x.Word == v.Word).First().Weight += PosChange;
                            }
                            else
                            {
                                elements.Where(x => x.Word == v.Word).First().Accuracy = cor / count;
                            }
                        }
                    }
                    foreach(var v in elements)
                    {
                        XElement e = v.CreateXML();
                        output.Root.Add(e);
                    }
                    output.Save(OutputFile);
                }
            }
        }
    }
}
