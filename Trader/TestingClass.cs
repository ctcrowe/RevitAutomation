using System;
using System.Linq;
using Alpaca.Markets;
using System.Threading.Tasks;
using CC_Library;

namespace Trader
{
    class TestingClass
    {
        private static string API_KEY = "PK2CPPF4DJ29SX61712T";

        private static string API_SECRET = "0XJpuQJ5QamtvrdlMsjxFj3YFPQ2Kqp3yNh9PnVx";

        public static async Task Main()
        {

            Console.WriteLine("Test");
            // First, open the API connection
            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            Console.WriteLine("Test");

            var into = DateTime.Now;
            into = into.AddHours(-1);
            var from = into.AddDays(-3);

            Console.WriteLine("Test");
            try
            {
                var aaplbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", from, into, BarTimeFrame.Hour));
                var oneqbars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("ONEQ", from, into, BarTimeFrame.Hour));
                var vtibars = await client.ListHistoricalBarsAsync(new HistoricalBarsRequest("VTI", from, into, BarTimeFrame.Hour));
                var items = aaplbars.Items;

                Console.WriteLine("Test");

                for (int i = 0; i < items.Count(); i++)
                {
                    Console.WriteLine("Bar Number : " + i);
                    Console.WriteLine("High : " + (double)items[i].High);
                    Console.WriteLine("Low : " + (double)items[i].Low);
                    Console.WriteLine();
                }
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
