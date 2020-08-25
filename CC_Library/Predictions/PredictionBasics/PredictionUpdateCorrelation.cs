using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class PredictionUpdateCorrelation
    {
        public static void SetCorrelation(this Dictionary<string, Element> dataset, int[] Correlation)
        {
            foreach (Element e in dataset.Values)
                e.Referencing = Correlation;
        }
        public static void UpdateCorrelation(this Dictionary<string, Element> dataset, int[] Correlation)
        {
            foreach(Element e in dataset.Values)
            {
                e.Referencing = Correlation;
                e.Write();
            }
        }
    }
}