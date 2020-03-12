using System.Linq;

namespace CC_Library.Predictions
{
    public static class FindResultantVector
    {
        public static string ResultantVector(this string s, DataFile df)
        {
            var title = s.SplitTitle();
            Data vector = new Data("result");

            for(int i = 0; i < title.Count(); i++)
            {
                Data d = title[i].ToDataPoint();
                for(int j = 0; j < 20; j++)
                {
                    vector.SetValue(j, vector.GetValue(j) + d.GetValue(j));
                }
            }

            for(int i = 0; i < 20; i++)
            {
                vector.SetValue(i, vector.GetValue(i) / title.Count());
            }

            var results = df.GetDataSet();
            int r = 0;
            double r2 = double.MaxValue;
            for(int i = 0; i < results.Count(); i++)
            {
                double v = results[i].CalcDistance(vector);
                if(v < r2)
                {
                    r = i;
                    r2 = v;
                }
            }
            return results[r].Phrase;
        }
    }
}
