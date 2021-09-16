using System;
using Alpaca.Markets;
using System.Threading.Tasks;

namespace Trader
{
    class TestingClass
    {
        private static string API_KEY = "PK2CPPF4DJ29SX61712T";

        private static string API_SECRET = "0XJpuQJ5QamtvrdlMsjxFj3YFPQ2Kqp3yNh9PnVx";

        public static async Task Main()
        {
            // First, open the API connection
            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaTradingClient(new SecretKey(API_KEY, API_SECRET));

            // Get our account information.
            var account = await client.GetAccountAsync();

            // Check if our account is restricted from trading.
            if (account.IsTradingBlocked)
            {
                Console.WriteLine("Account is currently restricted from trading.");
            }

            Console.WriteLine(account.BuyingPower + " is available as buying power.");

            Console.Read();
        }
    }
}
