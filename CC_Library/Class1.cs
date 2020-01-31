
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;
        /*
        internal class Prediction
        {
            public string Phrase { get; }
            public List<PredOption> Options { get; set; }
        
            public Prediction(string p)
            {
                this.Phrase = p;
                this.Options = new List<PredOption>();
            }
        }
        internal class PredOption
        {
            public string Name { get; }
            public double Adjustment { get; set; }
            public int Count { get; set; }
            
            public PredOption(string Word)
            {
                this.Name = Word;
                this.Adjustment = 0;
                this.Count = 1;
            }
            public XElement CreateOption()
            {
            }
        }
        */
namespace CC_Library
{
    public class TitleAnalysisPrediction
    {
        public delegate void TEST(string s);
        private static XDocument XDoc
        {
            get
            {
                Assembly a = typeof(TitleAnalysisPrediction).Assembly;

                string name = a.GetManifestResourceNames().Where(x => x.EndsWith("1.xml")).First();
                Stream s = a.GetManifestResourceStream(name);
                XDocument doc = XDocument.Load(s);
                //s.Close();
                return doc;
            }
        }
        public static int GenPrediction(string Title, TEST t)
        {
            /*
            foreach(string s in title)
            {
                var e = new prediction"";
            
            }
            */
            List<string> words = TitleAnalysis.SplitTitleWords(Title);
            List<PredictionElement> preds = Predictions(words);
            double[] vals = TitleAnalysis.GetPrediction(preds);
            int p = Array.IndexOf(vals, vals.Max());
            return p;
        }
        private static List<PredictionElement> Predictions(List<string> Words)
        {
            List<PredictionElement> pes = new List<PredictionElement>();
            foreach (string s in Words)
            {
                XDocument doc = XDoc;
                if (doc.Root.Elements().Any(x => x.Attribute("Word").Value == s))
                {
                    XElement ele = doc.Root.Elements().Where(x => x.Attribute("Word").Value == s).First();
                    double[] values = new double[ele.Elements().Count()];
                    for (int i = 0; i < values.Count(); i++)
                    {
                        values[i] = double.Parse(ele.Elements().Where(x => x.Attribute("Number").Value == i.ToString()).First().Attribute("Value").Value);
                    }
                    PredictionElement pe = new PredictionElement(s, values);
                    pes.Add(pe);
                }
                else
                {
                    pes.Add(new PredictionElement(s));
                }
            }
            return pes;
        }
    }
}
