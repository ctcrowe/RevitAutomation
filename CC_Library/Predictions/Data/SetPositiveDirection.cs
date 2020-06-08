using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class Data_SetPositiveDirection
    {
        public static void SetPositiveDirection
            (this KeyValuePair<string, Data> Datum,
            Dictionary<string, Data> ComparativeDataset,
            Dictionary<string, string[]> EntrySet)
        {
            Dictionary<string, string[]> ReducedEntries = Datum.ReducedEntries(EntrySet);
            Dictionary<string, Data> Dataset;
            if (Datum.Value.Datatype == Datatypes.Datatype.Dictionary)
                Dataset = ComparativeDataset.Where(x => ReducedEntries.Any(y => y.Value.Any(z => z == x.Key)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            else
                Dataset = ComparativeDataset.Where(x => ReducedEntries.Any(y => y.Key.SplitTitle().Any(z => z == x.Key)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            double[] results = new double[Datum.Value.Location.Count()];
            double[] Location = new double[Datum.Value.Location.Count()];

            foreach (var Reduced in Dataset)
            {
                double distance = Datum.CalcDistance(Reduced);
                for (int i = 0; i < Location.Count(); i++)
                    Location[i] += Reduced.Value.Location[i] * distance;
            }
            Location.Divide(Dataset.Count());

            var normal = Location.NormalizeVector();

            Datum.Value.PositiveDirection = normal;
        }
    }
}
