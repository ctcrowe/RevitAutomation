
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;

/*
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CuratedData";
        
        string attb = "MFSection"
        string ID = "Name"
        
        GetData(string Dataset, string ID, string Attb)
*/

namespace CC_Library
{
    internal class PredictionPhrase
    {
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

        public static List<PredictionPhrase> GetData(string Dataset, string ID, string Attb)
        {
            List<PredictionPhrase> data = new List<PredictionPhrase>();
            if(Directory.Exists(Dataset))
            {
                foreach(string f in Directory.GetFiles(Dataset))
                {
                    XDocument doc = XDocument.Load(f);
                    if(doc.Root.Attribute(ID) != null && doc.Root.Attribute(attb) != null)
                    {
                        data.Add(new PredictionPhrase(doc.Root.Attribute(ID).Value, doc.Root.Attribute(attb).Value));
                    }
                }
            }
            return data;
        }
    }
}
