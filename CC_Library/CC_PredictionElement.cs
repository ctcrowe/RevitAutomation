using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

namespace CC_Library
{
    public class PredictionElement
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string xfile = directory + "\\CC_XMLDictionary.xml";

        public const int PredictionCount = 48;
        public string Word { get; }
        public double[] Predictions { get; set; }

        public PredictionElement(string s)
        {
            this.Word = s;
            this.Predictions = new double[PredictionCount];
        }
        
        public void AdjustPredictions(double Value, int Guess, int Correct int Round)
        {
            if(Guess == Correct)
            {
                Predictions[Correct] += (0 / Round);
                for(int i = 0; i < Predictions.Count(); i++)
                {
                    if(i != Correct)
                        Predictions i -= (0 / (Round * PredictionCount));
                }
            }
            else
            {
                Predictions[Correct] += (0 / Round);
                for(int i = 0; i < Predictions.Count(); i++)
                {
                    if(i != Correct)
                       Predicitons[i] -= (0 / (Round * PredictionCount));
                }
            }
        }
        
        public static List<PredictionElement> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<PredictionElement>();

            if (File.Exists(xfile))
            {
                XDocument doc = XDocument.Load(xfile);
                foreach (XElement ele in doc.Root.Elements())
                {
                    data.Add(new PredictionElement(ele.Attribute("Value").Value));
                }
            }
            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        foreach (var pe in SplitTitle(ele))
                            if (!data.Any(x => x.Word == pe.Word))
                                data.Add(pe);
                    }
                }
            }
            return data;
        }
        
        public static List<PredictionElement> SplitTitle(string s)
        {
            var data = new List<PredictionElement>();
            int b = 0;
            char[] cs = s.ToCharArray();
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
    }
}
