using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Alpaca.Markets;
using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace DataAnalysis
{
    public static class CompBar
    {
        public static IBar GetComparable(this IReadOnlyList<IBar> bars)
        {
            var first = bars.First().TimeUtc;
            var time = new DateTime(
                first.Year,
                first.Month,
                first.Day,
                20, 30, 0);

            var bar = bars.First();
            var TimeCompare = time - bar.TimeUtc;
            foreach(var b in bars)
            {
                var tc = DateTime.Compare(time, b.TimeUtc) < 0 ? b.TimeUtc - time : time - b.TimeUtc;
                if(tc < TimeCompare)
                {
                    bar = b;
                    TimeCompare = tc;
                }
            }
            return bar;
        }
    }
}
