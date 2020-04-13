using System.Linq;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class FindResultantVector
    {
        public static DataPt ResultantVector(this CSVData cData, List<DataPt> PhraseData)
        {
            var title = cData.Name.SplitTitle();
            int titlecount = 0;
            DataPt vector = new DataPt("result");

            if (PhraseData.Any())
            {
                for (int i = 0; i < title.Count(); i++)
                {
                    if (PhraseData.Any(x => x.Phrase == title[i]))
                    {
                        titlecount++;
                        DataPt d = PhraseData.Where(x => x.Phrase == title[i]).First();
                        for (int j = 0; j < 20; j++)
                        {
                            vector.SetValue(j, vector.GetValue(j) + d.GetValue(j));
                        }
                    }
                }
            }

            if (titlecount > 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    vector.SetValue(i, vector.GetValue(i) / titlecount);
                }
            }
            return vector;
        }
    }
}
