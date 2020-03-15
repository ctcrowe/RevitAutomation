using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CC_Library.Predictions
{
    public static class CreateFiles
    {
        public static void CreateFolder(this DataFile df, CMDGetMyDocs.WriteOutput wo)
        {
            string folder = df.ToString().GetMyDocs(wo);
            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        public static void Create(this DataFile df, CMDGetMyDocs.WriteOutput wo)
        {
            List<Data> TextData = DataFile.TextData.GetDataSet();
            List<Data> VariableData = df.GetDataSet();
            Solution[] solutions = df.GetSolutions(wo);

            foreach(Solution s in solutions)
            {
                if (!TextData.Any(x => x.Phrase == s.DataName()))
                    TextData.Add(new Data(s.DataName()));
                if (!VariableData.Any(x => x.Phrase == s.SolutionValue()))
                    VariableData.Add(new Data(s.SolutionValue()));
            }

            while(true)
            {
                for(int i = 0; i < TextData.Count(); i++)
                {
                    TextData[i] = TextData.FullCompare(VariableData, solutions, i);
                }
                for(int i = 0; i < VariableData.Count(); i++)
                {
                    VariableData[i] = VariableData.FullCompare(TextData, solutions, i);
                }
                DataFile.TextData.Save(TextData, wo);
                df.Save(VariableData, wo);
            }
        }

        internal static double CalcAccuracy(this List<Data> dataset, Solution[] solutions, List<Data> VariableSet)
        {
            int total = 0;
            int correct = 0;
            
            foreach(Solution s in solutions)
            {
                total++;
                if(s.DataName().ResultantVector(dataset, VariableSet) == s.SolutionValue())
                    correct++;
            }
            return correct / total;
        }
        internal static Data FullCompare(this List<Data> dataset, List<Data> VariableSet, Solution[] solutions, int x)
        {
            Data d = dataset[x];
            for(int i = 0; i < 20; i++)
            {
                d.SetValue(i, dataset.Compare(solutions, VariableSet, x, i));
            }
            return d;
        }
        internal static int Compare(this List<Data> dataset, Solution[] solutions, List<Data> variableset, int x, int y)
        {
            int ivalue = 0;
            double acc = 0;
            for(int i = -10; i <= 10; i++)
            {
                List<Data> adjusted = dataset;
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
        internal static List<string> GetPhrases(this Solution[] sols)
        {
            List<string> phrases = new List<string>();
            foreach(Solution s in sols)
            {
                if (!phrases.Any(x => x == s.DataName()))
                    phrases.Add(s.DataName());
            }
            return phrases;
        }
    }
}
