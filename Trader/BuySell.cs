using System;
using System.Collections.Generic;
using System.Text;

namespace Trader
{
    class BuySell
    {
        private static string API_KEY = "PK2CPPF4DJ29SX61712T";

        private static string API_SECRET = "0XJpuQJ5QamtvrdlMsjxFj3YFPQ2Kqp3yNh9PnVx";

        public static async Task Main()
        {
            List<double> input = new List<double>();
            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            var into = DateTime.Now;
            into = into.AddHours(-1);
            var from = into.AddDays(-7);

            try
            {
                var aaplbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", from, into, BarTimeFrame.Hour));
                var qqqbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("QQQ", from, into, BarTimeFrame.Hour));
                var vtibars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("VTI", from, into, BarTimeFrame.Hour));
                var quote = await client.GetLatestQuoteAsync("AAPL");
                //Future version
                //double[] input = new double[###];
                //Parallel.For(0, aaplbars.Items.Count(); j =>
                //{
                //  input[j] = (double)aaplbars.Items[j].High;
                //  input[j + ##] = (double)aaplbars.Items[j].Low;
                //});
                for(int i = 0; i < aaplbars.Items.Count(); i++)
                {
                    input.Add((double)aaplbars.Items[i].High);
                    input.Add((double)aaplbars.Items[i].Low);
                }
                for(int i = 0; i < qqqbars.Items.Count(); i++)
                {
                    input.Add((double)qqqbars.Items[i].High);
                    input.Add((double)qqqbars.Items[i].Low);
                }
                for(int i = 0; i < vtibars.Items.Count(); i++)
                {
                    inputs.Add((double)vtibars.Items[i].High);
                    inputs.Add((double)vtibars.Items[i].Low);
                }
                inputs.Add((double)quote.AskPrice);
                inputs.Add((double)quote.BidPrice);

                Console.WriteLine("Test, " + inputs.Count());
            }
            catch(Exception e)
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
    }
}
