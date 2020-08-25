using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class FindBestResult
    {
        public static KeyValuePair<int, double[]> FindResult(this KeyValuePair<int, double[]>[] Possibilities)
        {
            double Step1 = Possibilities.Min(x => ((x.Value[3] * (1.66 - (x.Value[0] / x.Value[1]))) - (x.Value[2] * ((x.Value[0] / x.Value[1]) - 0.66))));
            var Step2 = Possibilities.Where(x => ((x.Value[3] * ((1.66 - x.Value[0] / x.Value[1]))) - (x.Value[2] * ((x.Value[0] / x.Value[1]) - 0.66))) <= Step1);
            var FinResult = Step2.OrderByDescending(x => x.Value[0]).ThenBy(y => y.Value[3]).First();
            return FinResult;
        }
    }
}
