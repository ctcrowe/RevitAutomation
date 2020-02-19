
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
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CuratedData";

        public string Phrase { get; }
        public string Prediction { get; set; }
        public List<string> Elements { get; set; }
                
        public PredictionPhrase(string i, string o)
        {
            this.Phrase = i;
            this.Prediction = o;
            this.Elements = i.SplitTitle();
        }
        public PredictionPhrase(string i)
        {
            this.Phrase = i;
            this.Elements = i.SplitTitle();
        }

        public static List<PredictionPhrase> GetData()
        {
            List<PredictionPhrase> data = new List<PredictionPhrase>();
            if(Directory.Exists(Dataset))
            {
                foreach(string f in Directory.GetFiles(Dataset))
                {
                    XDocument doc = XDocument.Load(f);
                    if(doc.Root.Attribute("Name") != null && doc.Root.Attribute("MFSection") != null)
                    {
                        data.Add(new PredictionPhrase(doc.Root.Attribute("Name").Value, doc.Root.Attribute("MFSection").Value));
                    }
                }
            }
            return data;
        }
    }
}
