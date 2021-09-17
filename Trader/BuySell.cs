﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Trader
{
    public class PriceData
    {
        public double[] Ask {get; set;}
        public double[] Bid {get; set;}
        public PriceData()
        {
            this.Ask = new double[50];
            this.Bid = new double[50];
        }
    }
    internal static class RecordData
    {
        
    }
    internal class BuySell
    {
        //private const int count = 1234;
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
                //double[] input = new double[5 * count + 2];
                //Parallel.For(0, aaplbars.Items.Count(); j =>
                //{
                //  input[j] = (double)aaplbars.Items[j].High;
                //  input[j + count] = (double)aaplbars.Items[j].Low;
                //  input[j + (2 * count)] = (double)qqqbars.Items[j].High;
                //  input[j + (3 * count)] = (double)qqqbars.Items[j].Low;
                //  input[j + (4 * count)] = (double)vtibars.Items[j].High;
                //  input[j + (5 * count)] = (double)vtibars.Items[j].Low;
                //});
                //  input[6 * count] = (double)quote.AskPrice;
                //  input[(6 * count) + 1] = (double)quote.BidPrice;
                
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
