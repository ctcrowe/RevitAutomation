using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CuratedData";
        
        string attb = "MFSection"
        string ID = "Name"
        
        GetData(string Dataset, string ID, string Attb)
        CalcAccuracy(this List<PredictionElement> Predictions, string Word, string Dataset, string ID, string Attb)
        
        RunAdjustment(this List<PredictionElement> elements,
            string Name,
            string dataset,
            string id,
            string attb,
            Write w)
*/

namespace CC_Library
{
    public class GetPredictions
    {
        public static void Run(Write w)
        {
            var PE = new List<PredictionElement>();
            var Phrases = PredictionPhrase.GetData();
            foreach(var p in Phrases)
            {
                foreach(string e in p.Elements)
                {
                    if (!PE.Any(x => x.Word == e))
                        PE.Add(new PredictionElement(e));
                    PE.Where(x => x.Word == e).First().AddOption(p.Prediction);
                }
            }
            foreach (var ele in PE)
            {
                foreach (var phrase in Phrases.Where(x => x.Phrase.Contains(ele.Word)))
                {
                    foreach(var p in phrase.Elements)
                    {
                        if(p != ele.Word)
                        {
                            ele.SubtractOption(phrase.Prediction);
                        }
                    }
                }
            }
            PE.RunAdjustment(w);
        }
    }
}
