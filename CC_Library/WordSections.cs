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
        private static void GenPredictionV2(string Entry, string Exit)
        {
            var Input = new List<Titleanalysis>();
            var Output = new List<PredictionElement>();
            
            string[] lines = File.ReadAllLines(Entry);
            foreach(string l in lines)
                Input.Add(new TitleAnalysis(l.Split('\t').First(), int.Parse(l.Split('\t')[2])));
            while(true)
            {
                foreach(var ta in Input)
                {
                    var Words = new List<PredictionElement>();
                }
            }
        }
        private static void GeneratePrediction(string Entry, string Exit)
        {
            var Input = new List<TitleAnalysis>();
            var Output = new List<PredictionElement>();

            string[] lines = File.ReadAllLines(Entry);
            foreach (string l in lines)
                Input.Add(new TitleAnalysis(l.Split('\t').First(), int.Parse(l.Split('\t')[2])));
            foreach(TitleAnalysis ta in Input)
            {
                foreach (var o in ta.SplitTitle())
                {
                    if (!Output.Any(x => x.Word == o.Word))
                        Output.Add(o);
                }
            }
            while(true)
            {
                XDocument xdoc = new XDocument(new XElement("PREDICTIONS")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                string fn = Exit.Split('.').First() + ".xml";
                for (int o = 0; o < Output.Count(); o++)
                {
                    foreach (var c in Input.Where(x => x.Title.Contains(Output[o].Word)).ToList())
                    {
                        double[] Prediction = c.GetPrediction(Output);
                        double max = Prediction.Max();
                        int p = Array.IndexOf(Prediction, max);
                        Output[o].Predictions = Output[o].AdjustPredictions(max, p, c.Section);
                    }
                }
                foreach(var o in Output)
                {
                    XElement e = new XElement("Prediction");
                    e.Add(new XAttribute("Word", o.Word));
                    for(int i = 1; i < o.Predictions.Count(); i++) 
                    {
                        XElement d = new XElement("Section");
                        d.Add(new XAttribute("Number", i.ToString()));
                        d.Add(new XAttribute("Value", o.Predictions[i].ToString()));
                        e.Add(d);
                    }
                    xdoc.Root.Add(new XElement(e));
                }
                xdoc.Save(fn);
            }
        }
        public static void run()
        {
            Command.Cmd RunAnalysis = new Command.Cmd(GeneratePrediction);
            Command.Run(RunAnalysis);
        }
    }
}
