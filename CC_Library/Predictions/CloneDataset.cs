using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class CloneDataset
    {
        public static Dataset Clone(this Dataset Base)
        {
            Dataset Copy = new Dataset(Base.datatype);
            foreach(KeyValuePair<string, double[]> data in Base.Data)
            {
                double[] values = new double[data.Value.Count()];
                for(int i = 0; i < values.Count(); i++)
                {
                    values[i] = data.Value[i];
                }
                Copy.Data.Add(data.Key, values);
            }
            return Copy;
        }
    }
}