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

        public double[,] StockValues(List<Item> items)
        {
            double[,] output = new double[items.Count(), 8];
            Parallel.For(0, items.Count(), j =>
                         {
                             output[j, 0] = items[J].Thing == "AAPL" ? 1 : 0;
                             output[j, 1] = items[J].Thing == "QQQ" ? 1 : 0;
                             output[j, 2] = items[J].Thing == "VTI" ? 1 : 0;
                             output[j, 3] = (items[j].Close - items[j].Open) / items[j].Close;
                             output[j, 4] = (items[j].High - items[j].Low) / items[j].Close;
                             output[j, 5] = (items[j].Close - items[j].Vwap) / items[j].Close;
                             output[j, 6] = items[j].Time - DateTime.Now();
                             output[j, 7] = items[j].Volume;
                         });
            return output;
        }
        
        public static async Task Main()
        {
            double[] input = new double[(6 * items) + 2];
            double[] outputs = new double[16];
            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            Random r = new Random();
            var rinto = r.Next(5, 1000);

            var into = DateTime.Now;
            into = into.AddDays(-rinto);
            var from = into.AddDays(-3);
            var output = into.AddDays(5);

            try
            {
                var aaplbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", from, into, BarTimeFrame.Hour));
                var qqqbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("QQQ", from, into, BarTimeFrame.Hour));
                var vtibars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("VTI", from, into, BarTimeFrame.Hour));
                var aaplout = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", into, output, BarTimeFrame.Hour));
                var quote = await client.GetLatestQuoteAsync("AAPL");
                var ask = (double)quote.AskPrice;
                var bid = (double)quote.BidPrice;
                var item = aaplbars.Items[0];
                var vol = (double)item.Volume;

                Parallel.For(0, aaplbars.Items.Count(), j =>
                {
                    input[j] = (double)aaplbars.Items[j].High;
                    input[j + items] = (double)aaplbars.Items[j].Low;
                    input[j + (2 * items)] = qqqbars.Items.Count() >= aaplbars.Items.Count() ? (double)qqqbars.Items[j].High : 0;
                    input[j + (3 * items)] = qqqbars.Items.Count() >= aaplbars.Items.Count() ? (double)qqqbars.Items[j].Low : 0;
                    input[j + (4 * items)] = vtibars.Items.Count() >= aaplbars.Items.Count() ? (double)vtibars.Items[j].High : 0;
                    input[j + (5 * items)] = vtibars.Items.Count() >= aaplbars.Items.Count() ? (double)vtibars.Items[j].Low : 0;
                });
                input[6 * items] = (double)quote.AskPrice;
                input[(6 * items) + 1] = (double)quote.BidPrice;

                for(int j = 0; j < 16; j++)
                {
                    outputs[j] = (double)aaplout.Items[j].Open;
                }

                Datatype.AAPL.PropogateSingle(input, outputs, new WriteToCMDLine(Write));
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
