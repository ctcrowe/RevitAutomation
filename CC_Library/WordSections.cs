using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace CC_Library
{
    public class WordSections
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string xfile = directory + "\\CC_XMLDictionary.xml";
        private const double Distance = 0.75;

        public static List<string> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<string>();

            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        foreach (var pe in PredictionElement.SplitTitle(ele))
                            if (!data.Contains(pe.Word))
                                data.Add(pe.Word);
                    }
                }
            }
            if (File.Exists(xfile))
            {
                XDocument doc = XDocument.Load(xfile);
                foreach (XElement ele in doc.Root.Elements())
                {
                    data.Add(ele.Attribute("Value").Value);
                }
            }
            return data;
        }
        public static void CopyToXml(string file, string n)
        {
            string[] lines = File.ReadAllLines(file);
            XDocument doc = new XDocument(new XElement("DICTIONARY")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            foreach (string s in lines)
            {
                XElement e = new XElement("string");
                e.Add(new XAttribute("Value", s));
            }
            doc.Save(xfile);
        }
        public static void GeneratePrediction(string Entry, string Exit)
        {
            var Input = new List<TitleAnalysis>();
            var Output = new List<PredictionElement>();

            string[] lines = File.ReadAllLines(Entry);
            foreach (string l in lines)
                Input.Add(new TitleAnalysis(l.Split('\t').First(), int.Parse(l.Split('\t')[2])));
            foreach(TitleAnalysis ta in Input)
            {
                Output.AddRange(ta.SplitTitle());
            }
            for(int k = 1; k < 1000; k++)
            {
                XDocument xdoc = new XDocument(new XElement("PREDICTIONS")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                string fn = Exit.Split('.').First() + "_" + k.ToString() + ".xml";
                foreach (var o in Output)
                {
                    int analysiscount = 1;
                    foreach (var c in Input.Where(x => x.Title.Contains(o.Word)).ToList())
                    {
                        double[] Prediction = c.GetPrediction(Output);
                        double m = Prediction.Max();
                        int p = Array.IndexOf(Prediction, m);
                        if(p == c.Section)
                        {
                            if(m < Distance)
                            {
                                o.Predictions[c.Section] += (Math.Abs(m - Distance) * ;
                                for (int i = 0; i < Prediction.Count(); i++)
                                {
                                    if (i != c.Section)
                                        Prediction[i] -= (Math.Abs(m - Distance) / PredictionElement.PredictionCount);
                                }
                            }
                        }
                        else
                        {
                            Prediction[c.Section] += Math.Abs(m - Distance);
                            for(int b = 0; b < Prediction.Count(); b++)
                                if(b != c.Section)
                                    Prediction[b] -= (Math.Abs(m-Distance) / Prediction.Count());
                        }
                    }
                }
                foreach(var o in Output)
                {
                    XElement e = new XElement("Prediction");
                    e.Add(new XAttribute("Word", o.Word));
                    for(int i = 0; i < o.Predictions.Count(); i++) 
                    {
                        int j = i + 1;
                        XElement d = new XElement("Section");
                        d.Add(new XAttribute("Number", j.ToString()));
                        d.Add(new XAttribute("Value", o.Predictions[i].ToString()));
                        e.Add(d);
                    }
                    xdoc.Root.Add(new XElement(e));
                }
                xdoc.Save(fn);
                k++;
            }
        }
        public static void run()
        {
            Command.Cmd RunAnalysis = new Command.Cmd(GeneratePrediction);
            Command.Run(RunAnalysis);
        }
    }
}
