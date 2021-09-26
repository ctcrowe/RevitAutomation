using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Alpaca.Markets;
using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace Trader
{
    class TestingClass
    {
        private const int items = 80;
        private static string API_KEY = "PK2CPPF4DJ29SX61712T";
        private static string API_SECRET = "0XJpuQJ5QamtvrdlMsjxFj3YFPQ2Kqp3yNh9PnVx";

        public static double[,] StockValues(List<IBar> items, DateTime dt)
        {
            double[,] output = new double[items.Count(), 8];
            Parallel.For(0, items.Count(), j =>
            {
                output[j, 0] = items[j].Symbol == "AAPL" ? 1 : 0;
                output[j, 1] = items[j].Symbol == "QQQ" ? 1 : 0;
                output[j, 2] = items[j].Symbol == "VTI" ? 1 : 0;
                output[j, 3] = ((double)items[j].Close - (double)items[j].Open) / (double)items[j].Close;
                output[j, 4] = ((double)items[j].High - (double)items[j].Low) / (double)items[j].Close;
                output[j, 5] = ((double)items[j].Close - (double)items[j].Vwap) / (double)items[j].Close;
                var ts = dt - items[j].TimeUtc;
                output[j, 6] = ts.TotalHours;
                output[j, 7] = items[j].Volume;
            });
            return output;
        }
        
        public static async Task Main()
        {
            double[] input = new double[(6 * items) + 2];
            double[] outputs = new double[2];
            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            Random r = new Random();
            var rinto = r.Next(5, 1000);

            var into = DateTime.Now;
            into = into.AddDays(-rinto);
            var from = into.AddDays(-3);
            var outto = into.AddHours(1.5);

            try
            {
                var aaplbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", from, into, BarTimeFrame.Hour));
                var qqqbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("QQQ", from, into, BarTimeFrame.Hour));
                var vtibars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("VTI", from, into, BarTimeFrame.Hour));
                var aaploutput = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", into, outto, BarTimeFrame.Hour));
                if (aaploutput.Items.Any())
                {

                    var bars = aaplbars.Items.ToList();
                    bars.AddRange(qqqbars.Items);
                    bars.AddRange(vtibars.Items);

                    Sample s = new Sample(Datatype.AAPL);
                    s.MktVals = StockValues(bars, aaploutput.Items.First().TimeUtc);
                    s.DesiredOutput = new double[2]
                    {
                    (double)aaploutput.Items.First().Open > (double)aaploutput.Items.First().Close ? 0 : 1,
                    (double)aaploutput.Items.First().Open > (double)aaploutput.Items.First().Close ? 1 : 0
                    };
                    AppleNetwork net = new AppleNetwork(s);
                    net.Propogate(s, new WriteToCMDLine(Write));
                }
            }
            catch(Exception e)
            {
                e.OutputError();
            }

            Console.Read();
        }
        public static string Write(string s)
        {
            Console.WriteLine(s);
            return s;
        }
    }
}
