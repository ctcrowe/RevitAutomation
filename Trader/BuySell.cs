﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Alpaca.Markets;
using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

//ToDo : Generate samples of value prediction using bars (similar to before) only feed it into a list of stonk values
// Then take the first n samples of that list and predict the high / low using the n samples from the list.
namespace Trader
{
    internal class BuySell
    {
        private static string API_KEY = "PK2CPPF4DJ29SX61712T";
        private static string API_SECRET = "0XJpuQJ5QamtvrdlMsjxFj3YFPQ2Kqp3yNh9PnVx";
        
        private static async List<StonkValues> GetBars(DateTime dt)
        {
            DateTime date = new DateTime(dt.Year, dt.Month, dt.Day);
        }
        private static StonkValues GetValues(IBar bar)
        {
            return null;
        }
        private static StonkValues GetValues(IQuote quote)
        {
            StonkValues vals = new StonkValues(quote.Symbol, quote.TimestampUtc,
                                   (double)quote.AskPrice, (double)quote.AskSize,
                                   (double)quote.BidPrice, (double)quote.BidSize);
            vals.Save();
            return vals;
        }

        public static async Task GetMarketData(List<StonkValues> vals)
        {
            Console.WriteLine("Test");
            var DClient = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));
            var TClient = Alpaca.Markets.Environments.Paper
                .GetAlpacaTradingClient(new SecretKey(API_KEY, API_SECRET));

            var clock = await TClient.GetClockAsync();
            
            if (clock.IsOpen)
            {
                AppleNetwork net = new AppleNetwork();
                    
                var AAPLQuote = GetValues(await DClient.GetLatestQuoteAsync("AAPL"));
                var QQQQuote = GetValues(await DClient.GetLatestQuoteAsync("QQQ"));
                var VTIQuote = GetValues(await DClient.GetLatestQuoteAsync("VTI"));
                    
                try
                {
                    if(vals.Count() > 6)
                    {
                        if(vals.Any(x => x.Symbol == "AAPL"))
                        {
                            var val = vals.Where(x => x.Symbol == "AAPL").OrderByDescending(x => x.Time).First();
                            bool inc = (AAPLQuote.AskPrice + AAPLQuote.BidPrice) > (val.AskPrice + val.BidPrice);
                            net.Propogate(vals, inc, new WriteToCMDLine(Write));
                            if(inc)
                                Write("Result : Buy");
                            else
                                Write("Result : Sell");
                        }
                    }
                }
                catch (Exception e) { e.OutputError(); }
                
                vals.Add(AAPLQuote);
                vals.Add(QQQQuote);
                vals.Add(VTIQuote);
                
                try
                {
                    if(vals.Count() > 6)
                    {
                        if(vals.Any(x => x.Symbol == "AAPL"))
                        {
                            var prediction = net.Predict(vals);
                            if(prediction == 0)
                                Write("Prediction : Buy");
                            else
                                Write("Prediction : Sell");
                        }
                    }
                }
                catch (Exception e)
                {
                    e.OutputError();
                }

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
                /*
                var into = DateTime.Now;
                into = into.AddHours(-1);
                var from = into.AddDays(-7);
                Random r = new Random();
                var rinto = r.Next(5, 1000);
                var testinto = from.AddDays(-rinto);
                var testfom = testinto.AddDays(-7);
                //for training data : inputs are going to be the data from 1 week prior starting at 2 days prior.
                //for training data : outputs are going to be the data from the past 2 days (8 bars total).

                try
                {
                    var aaplbars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", from, into, BarTimeFrame.Hour));
                    var qqqbars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("QQQ", from, into, BarTimeFrame.Hour));
                    var vtibars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("VTI", from, into, BarTimeFrame.Hour));
                    var quote = await DClient.GetLatestQuoteAsync("AAPL");
                    var barstest = aaplbars.Items.First().Open;
                    Console.WriteLine("Open Test : " + barstest);
                    
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

                    //Datatype.AAPL.PropogateSingle(input, outputs, new WriteToCMDLine(Write));
                    var Prediction = Datatype.AAPL.PredictMulti(input, new WriteToCMDLine(CMDLibrary.WriteNull));

                    if (!Prediction.Any(x => x <= (double)quote.AskPrice))
                    {
                        Console.WriteLine("Buy");
                    }
                    else if (!Prediction.Any(x => x >= (double)quote.AskPrice))
                    {
                        Console.WriteLine("Sell");
                    }
                    else
                    {
                        Console.WriteLine("Hold");
                    }
                }
                catch (Exception e)
                {
                    e.OutputError();
                }*/

                // Get our account information.

                // Check if our account is restricted from trading.
                /*
                if (account.IsTradingBlocked)
                {
                    Console.WriteLine("Account is currently restricted from trading.");
                }
                Console.WriteLine(account.BuyingPower + " is available as buying power.");
                */
