using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using CC_Library;

namespace CC_Library.Predictions
{
    public static class CCPredictionLoop
    {
        public static void RunPredictions(this Datatype dt, WriteToCMDLine wo)
        {
            List<DataPt> TextData = Datatype.TextData.GetDataSet();
            List<DataPt> VariableData = dt.GetDataSet();
            List<CSVData> solutions = dt.GetData();

            foreach (CSVData s in solutions)
            {
                var names = s.Name.SplitTitle();
                wo(s.Name);
                foreach (string n in names)
                {
                    if (!TextData.Any(x => x.Phrase == n))
                        TextData.Add(new DataPt(n));
                }
                if (!VariableData.Any(x => x.Phrase == s.Data))
                    VariableData.Add(new DataPt(s.Data));
            }

            while (true)
            {
                for (int i = 0; i < TextData.Count(); i++)
                {
                    TextData[i] = TextData.FullCompare(VariableData, solutions, i);
                }
                for (int i = 0; i < VariableData.Count(); i++)
                {
                    VariableData[i] = VariableData.FullCompare(TextData, solutions, i);
                }
                Datatype.TextData.Save(TextData, wo);
                dt.Save(VariableData, wo);
            }
        }
    }
}
