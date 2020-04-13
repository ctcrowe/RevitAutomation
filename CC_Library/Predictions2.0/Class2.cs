using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class CreateFiles
    {
        internal static DataPt GetClosestPoint(this DataPt d, List<DataPt> Dataset)
        {
            double x = double.MaxValue;
            int j = int.MaxValue;
            for(int i = 0; i < Dataset.Count(); i++)
            {
                double dist = d.CalcDistance(Dataset[i]);
                if(dist < x)
                {
                    j = i;
                    x = dist;
                }
            }
            return Dataset[j];
        }
        internal static double CalcAccuracy(this List<DataPt> PhraseData, List<CSVData> solutions, List<DataPt> ParameterData)
        {
            int total = 0;
            int correct = 0;
            
            foreach(CSVData s in solutions)
            {
                DataPt dp = s.ResultantVector(PhraseData);
                total++;
                if (dp.GetClosestPoint(ParameterData).Phrase == s.Data)
                    correct++;
            }
            return correct / total;
        }
        internal static DataPt FullCompare(this List<DataPt> dataset, List<DataPt> VariableSet, List<CSVData> solutions, int x)
        {
            DataPt d = dataset[x];
            for(int i = 0; i < 20; i++)
            {
                d.SetValue(i, dataset.Compare(solutions, VariableSet, x, i));
            }
            return d;
        }
        internal static int Compare(this List<DataPt> dataset, List<CSVData> solutions, List<DataPt> variableset, int x, int y)
        {
            int ivalue = 0;
            double acc = 0;
            for(int i = -10; i <= 10; i++)
            {
                List<DataPt> adjusted = dataset;
                int val = dataset[x].GetValue(y);
                adjusted[x].SetValue(y, val + i);
                double newacc = adjusted.CalcAccuracy(solutions, variableset);
                if(newacc > acc)
                {
                    ivalue = i;
                    acc = newacc;
                }
            }
            return ivalue;
        }
        internal static List<string> GetPhrases(this List<CSVData> sols)
        {
            List<string> phrases = new List<string>();
            foreach(CSVData s in sols)
            {
                if (!phrases.Any(x => x == s.Name))
                    phrases.Add(s.Name);
            }
            return phrases;
        }
    }
}
