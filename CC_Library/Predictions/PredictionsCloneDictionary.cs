using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class PredictionsCloneDictionary
    {
        public static Dictionary<string, Data> CloneDictionary(
            this Dictionary<string, Data> Dictionary, int[] PairedLocation,
            WriteToCMDLine write)
        {
            Dictionary<string, Data> NewDict = Dictionary.Clone();
            foreach(KeyValuePair<string, Data> pairs in NewDict)
            {
                double[] values = new double[PairedLocation.Count()];
                for(int i = 0; i < PairedLocation.Count(); i++)
                {
                    values[i] = pairs.Value.Location[PairedLocation[i]];
                }
                pairs.Value.Location = values;
                pairs.Value.RefPoints = PairedLocation;
            }
            return NewDict;
        }
    }
}
