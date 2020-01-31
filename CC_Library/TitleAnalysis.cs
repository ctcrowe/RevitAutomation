using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;

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
        public static int GenPrediction(string Title, TEST t)
        {
            List<string> words = TitleAnalysis.SplitTitleWords(Title);
            List<PredictionElement> preds = Predictions(words);
            double[] vals = TitleAnalysis.GetPrediction(preds);
            int p = Array.IndexOf(vals, vals.Max());
            return p;
        }
    }
    public class TitleAnalysis
    {
        public string Title { get; }
        public int Section { get; }

        public TitleAnalysis(string s, int i)
        {
            this.Title = s;
            this.Section = i;
        }
        
        public static double[] GetPrediction(List<PredictionElement> PredictionWords)
        {
            double[] Prediction = new double[PredictionElement.PredictionCount];
            for(int z = 0; z < Prediction.Count(); z++)
            {
                double a = 0;
                foreach(var x in PredictionWords)
                {
                    double v = x.Predictions[z] * x.Predictions[z];
                    a += v;
                }
                a /= PredictionWords.Count();
                Prediction[z] = Math.Sqrt(a);
            }
            return Prediction;
        }
        public static List<string> SplitTitleWords(string Title)
        {
            var data = new List<string>();
            int b = 0;
            char[] cs = Title.ToCharArray();
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
                        data.Add(z);
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
                            data.Add(z);
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
        public List<string> SplitTitleWords()
        {
            var data = new List<string>();
            int b = 0;
            char[] cs = this.Title.ToCharArray();
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
                        data.Add(z);
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
                            data.Add(z);
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
        public List<PredictionElement> SplitTitle()
        {
            var data = new List<PredictionElement>();
            int b = 0;
            char[] cs = this.Title.ToCharArray();
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
                        data.Add(new PredictionElement(z));
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
                            data.Add(new PredictionElement(z));
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
        public static List<TitleAnalysis> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<TitleAnalysis>();

            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        if (!data.Any(x => x.Title == ele))
                        {
                            if(doc.Root.Attribute("Section") != null)
                            {
                                data.Add(new TitleAnalysis(ele, int.Parse(doc.Root.Attribute("Section").Value)));
                            }
                            else
                            {
                                data.Add(new TitleAnalysis(ele, 0));
                            }
                        }
                    }
                }
            }
            return data;
        }
    }
}
