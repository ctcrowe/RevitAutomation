using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

namespace CC_Library
{
    public delegate void Write(string s);
    public class AdjustPredictions
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
        internal static void Run(List<PredictionElement> elements, Write w)
        {
            /*
            int count = 0;
            int cor = 0;
            int cor2 = 0;
            int cor3 = 0;*/
                
            List<PredictionPhrase> phrases = PredictionPhrase.GetData();
            
            While(true)
            {
                foreach(var e in elements)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        elements.AdjustWeight(e.Word, i);
                    }
                    w(e.Weight.ToString());
                }
            }
            #region oldcode
            /*
            while (elements.Any(x => x.Accuracy < 1))
            {
                string s = "";
                foreach (var v in elements.Where(x => x.Accuracy < 1))
                {
                    cor = 0;
                    cor2 = 0;
                    cor3 = 0;
                    count = 0;
                    double poscw = Math.Abs(1 - v.Weight);
                    double negcw = Math.Abs(0 - Math.Abs(v.Weight));
                    double mca = 1 - v.Accuracy;
                    double NegChange = negcw * mca / 2;
                    double PosChange = poscw * mca / 2;
                    if (NegChange == 0)
                        NegChange += 0.01;
                    if (PosChange == 0)
                        PosChange += 0.01;

                    var eleNeg = elements;
                    var elePos = elements;
                    eleNeg.Where(x => x.Word == v.Word).First().Weight -= NegChange;
                    elePos.Where(x => x.Word == v.Word).First().Weight += PosChange;

                    foreach (var p in phrases.Where(x => x.Phrase.Contains(v.Word)))
                    {
                        var PhraseElements = elements.Where(x => p.Elements.Any(y => y == x.Word));
                        var PENeg = eleNeg.Where(x => p.Elements.Any(y => y == x.Word));
                        var PEPos = elePos.Where(x => p.Elements.Any(y => y == x.Word));
                        count++;
                        if (RunFormula(PhraseElements.ToList()) == p.Prediction)
                            cor++;
                        if (RunFormula(PENeg.ToList()) == p.Prediction)
                            cor2++;
                        if (RunFormula(PEPos.ToList()) == p.Prediction)
                            cor3++;
                    }
                    if (cor2 > cor3 && cor2 >= cor)
                    {
                        elements.Where(x => x.Word == v.Word).First().Accuracy = cor2 / count;
                        elements.Where(x => x.Word == v.Word).First().Weight -= NegChange;
                    }
                    else
                    {
                        if (cor3 > cor2 && cor3 >= cor)
                        {
                            elements.Where(x => x.Word == v.Word).First().Accuracy = cor3 / count;
                            elements.Where(x => x.Word == v.Word).First().Weight += PosChange;
                        }
                        else
                        {
                            elements.Where(x => x.Word == v.Word).First().Accuracy = cor / count;
                        }

                        s += v.Word + " " + v.Accuracy.ToString() + " : ";
                    }
                    w(s);
                }
            }*/
            #endregion
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
