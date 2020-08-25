using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class PredictStudLayer
    {
        public static string PredictStudSize(this string[] Words)
        {
            Element e = Datatype.StudLayer.GetElement("Stud");

            List<double[]> Positions = new List<double[]>();
            foreach (string Phrase in Words)
            {
                var WordList = Phrase.SplitTitle();
                Dictionary<string, Element> DictPoints = new Dictionary<string, Element>();
                foreach (var word in WordList)
                {
                    if (!DictPoints.ContainsKey(word))
                        DictPoints.Add(word, Datatype.Dictionary.GetElement(word));
                }
                Positions.Add(DictPoints.Combine());
            }
            int closest = Positions.FindClosest(e);
            return Words.ElementAt(closest);
        }
    }
}