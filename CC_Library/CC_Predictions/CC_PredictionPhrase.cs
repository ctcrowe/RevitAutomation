using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

/*
Version 2.0
-Every Prediction will be assessed on a 20 dimensional unknown parameter array.
-The sequence will work as follows
        1) set all assessments to their existing values in the library
        2) Add new values
        3) Run a prediction on the given values
                3a) iterate through dimensional options (from -100 to 100) to see where a phrase is best fit.
                3b) iterate through dimensional options (from -100 to 100) to see where solutions best fit.
                3c) accept the new locations of the words and results as 2 separate files that control how to resolve a prediction
        4) Word 2 Vec style analysis.
        5) each predictive result then exists as a set of 20 dimensional arrays
        6) any input can be judged on a 20 dimensional array if any of the words are in the array.
-Words can then be judged as a distance and as a closest, using basic geometry.
-the computer then can find similarities between objects, rather than just a guess at a set of options.
-This will also let us draw conclusions that havent been dealt with before, by cross contaminating datasets to solution sets.
-The first dataset will initialize everything to 0 and everything will hopefully expand away from 0. If something is too close to 0, its accuracy is lower.
-this function will effectively be able to create solutions with minimal background (hopefully) by modifying the existing values over time.

internal class PredictionArray
{
        private var Data = new Dictionary<string, double[]>();
        private var Solution = new Dictionary<string, double[]>();
        
}
*/

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

        public static List<PredictionPhrase> GetData(string Dataset, string ID, string attb)
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
