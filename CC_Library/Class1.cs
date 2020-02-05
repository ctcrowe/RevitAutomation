
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;

namespace CC_Library
{
    internal class PredictionPhrase
        {
            public string Phrase { get; }
            public List<PredictionElement> Elements { get; set; }
            public int Prediction { get; set; }
                
            public PredictionPhrase(string p)
            {
                this.Phrase = p;
                this.Elements = GetElements();
                this.Prediction = 0;
            }
            private List<PredictionElement> GetElements()
            {
                var Elements = new List<PredictionElement>();
                int b = 0;
                char[] cs = Phrase.ToCharArray();
                for (int i = 1; i < cs.Count(); i++)
                {
                    if (!char.IsLetter(cs[i]))
                    {
                        if (i > b && b < cs.Count())
                        {
                            string z = string.Empty;
                            for (int j = b; j < i; j++)
                            {
                                z += cs[j];
                            }
                            Elements.Add(new PredictionElement(z));
                        }
                        b = i + 1;
                    }
                    else
                    {
                        if (char.IsUpper(cs[i]) && !char.IsUpper(cs[i - 1]))
                        {
                            if (i > b && b < cs.Count())
                            {
                                string z = string.Empty;
                                for (int j = b; j < i; j++)
                                {
                                    z += cs[j];
                                }
                                //data.Add(z);
                            }
                            b = i;
                        }
                    }
                }
                return Elements;
            }
        }
        internal class PredictionElement
        {
            public string Word { get; }
            internal List<PredOption> Options { get; set; }
        
            public PredictionElement(string w)
            {
                this.Word = w;
                this.Options = new List<PredOption>();
            }
            public PredictionElement(XElement ele)
            {
                this.Word = ele.Attribute("WORD").Value;
                this.Options = new List<PredOption>();
                foreach(XElement e in ele.Elements("OPTION"))
                {
                    Options.Add(new PredOption(e));
                }
            }
            public XElement CreateElement()
            {
                XElement ele = new XElement("ELEMENT");
                ele.Add(new XAttribute("WORD", Word));
                foreach(PredOption p in Options)
                {
                    ele.Add(p.CreateOption());
                }
                return ele;
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
            public PredOption(XElement ele)
            {
                this.Name = ele.Attribute("NAME").Value;
                this.Adjustment = double.Parse(ele.Attribute("ADJUSTMENT").Value);
                this.Count = int.Parse(ele.Attribute("QTY").Value);
            }
            public XElement CreateOption()
            {
                XElement ele = new XElement("OPTION");
                ele.Add(new XAttribute("NAME", this.Name));
                ele.Add(new XAttribute("ADJUSTMENT", this.Adjustment.ToString()));
                ele.Add(new XAttribute("QTY", this.Count.ToString()));
                return ele;
            }
        }
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
            List<PredElement> preds = Predictions(words);
            double[] vals = TitleAnalysis.GetPrediction(preds);
            int p = Array.IndexOf(vals, vals.Max());
            return p;
        }
        private static List<PredElement> Predictions(List<string> Words)
        {
            List<PredElement> pes = new List<PredElement>();
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
                    PredElement pe = new PredElement(s, values);
                    pes.Add(pe);
                }
                else
                {
                    pes.Add(new PredElement(s));
                }
            }
            return pes;
        }
    }
}
