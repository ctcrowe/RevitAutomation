using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alpaca.Markets;
using CC_Library;

namespace Trader
{
    public class PriceData
    {
        public List<double> Ask {get; set;}
        public List<double> Bid {get; set;}
        public PriceData()
        {
            this.Ask = new List<double>();
            this.Bid = new List<double>();
        }
    }
    internal static class RecordData
    {
        public static void Run()
        {

        }
    }
    internal class BuySell
    {
        private const int items = 80;
        private static string API_KEY = "PK2CPPF4DJ29SX61712T";
        private static string API_SECRET = "0XJpuQJ5QamtvrdlMsjxFj3YFPQ2Kqp3yNh9PnVx";


        public static async Task GetMarketData()
        {
            double[] input = new double[(6 * items) + 2];
            var DClient = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));
            var TClient = Alpaca.Markets.Environments.Paper
                .GetAlpacaTradingClient(new SecretKey(API_KEY, API_SECRET));

            var clock = await TClient.GetClockAsync();

            if (clock.IsOpen)
            {
                var into = DateTime.Now;
                into = into.AddHours(-1);
                var from = into.AddDays(-7);

                try
                {
                    var aaplbars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", from, into, BarTimeFrame.Hour));
                    var qqqbars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("QQQ", from, into, BarTimeFrame.Hour));
                    var vtibars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("VTI", from, into, BarTimeFrame.Hour));
                    var quote = await DClient.GetLatestQuoteAsync("AAPL");


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

                    string s = input[0].ToString();
                    for (int i = 1; i < input.Count(); i++)
                    {
                        s += ", " + input[i];
                    }
                    Console.WriteLine(s);
                }
                catch (Exception e)
                {
                    e.OutputError();
                }

                // Get our account information.

                // Check if our account is restricted from trading.
                /*
                if (account.IsTradingBlocked)
                {
                    Console.WriteLine("Account is currently restricted from trading.");
                }
                Console.WriteLine(account.BuyingPower + " is available as buying power.");
                */
                Console.Read();
            }
            else
            {
                Console.WriteLine("Market Closed");
                Console.Read();
            }
        }
    }
}
