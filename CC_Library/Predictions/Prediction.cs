using System.Collections.Generic;
using System.Linq;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class Prediction
    {
        public static string FindClosest(this Datatype type, string phrase)
        {
            Dataset dataset = type.LoadDataset();
            Dataset dictionary = Datatype.TextData.LoadDataset();
            if (dataset.Data.Any())
            {
                List<string> words = phrase.SplitTitle();

                IEnumerable<KeyValuePair<string, double[]>> datum = dictionary.Data.Where(x => words.Contains(x.Key));
                if (datum.Any())
                {
                    KeyValuePair<string, double[]> data = datum.ResultantDatapoint();
                    return dataset.FindClosest(data).Key;
                }
            }
            return null;
        }
    }
}