using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace CC_Library
{
    public class GenPredictions
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CC_XMLData";
        private static string FileName = directory + "\\CC_MFData.xml";
        
        public static void Run()
        {
            if(Directory.Exists(Dataset))
            {
                List<PredictionElement> pe = new List<PredictionElement>();
                XDocument xfinsihed = new XDocument(new XElement("MASTERFORMAT")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                
                string[] files = Directory.GetFiles(folder);
                foreach(string f in files)
                {
                    XDocument doc = XDocument.Load(f);
                    if(doc.Root.Attribute("MFSection") != null)
                    {
                        string Datapoint = doc.Root.Attribute("MFSection").Value;
                        List<string> Elements = SplitTitle.Run(doc.Root.Attribute("Name").Value);
                        foreach(string e in Elements)
                        {
                            if(pe.Any(x => x.Word == e))
                            {
                                pe.Where(x => x.Word == e).First().AddOption(Datapoint);
                            }
                            else   
                            {
                                var o = new PredictionElement(e);
                                o.AddOption(Datapoint);
                                pe.Add(o);
                            }
                        }
                    }
                }
                foreach(var p in pe) { p.SetRatios(); }
                double accuracy = 0;
                int correct = 0;
                int wrong = 0;
                while(true)
                {
                    List<PredictionElement> pe2 = pe;
                    foreach(var p in pe)
                    {
                    }
                }
            }
        }
    }
}
