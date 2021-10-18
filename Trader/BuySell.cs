using System;
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
        
        private static StonkValues GetValues(IQuote quote)
        {
            StonkValues vals = new StonkValues(quote.Symbol, quote.TimestampUtc,
                                   (double)quote.AskPrice,
                                   (double)quote.BidPrice);
            return vals;
        }
        private static StonkValues GetValues(IBar bar)
        {
            StonkValues vals = new StonkValues(bar.Symbol, bar.TimeUtc, (double)bar.Open);
            return vals;
        }
        public static async Task GetMarketData(List<StonkValues> vals)
        {
            AppleNetwork net = new AppleNetwork();

            var DClient = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));
            var TClient = Alpaca.Markets.Environments.Paper
                .GetAlpacaTradingClient(new SecretKey(API_KEY, API_SECRET));

            var clock = await TClient.GetClockAsync();
            
            try
            {
                Random r = new Random();
                var now = DateTime.Now;
                var into = new DateTime(now.Year, now.Month, now.Day);
                var from = into.AddDays(-1 * r.Next(1, 500));
                while (from.DayOfWeek == DayOfWeek.Sunday || from.DayOfWeek == DayOfWeek.Saturday)
                    from.AddDays(1);
                into = from.AddDays(1);
                
                var aaplbars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("AAPL", from, into, BarTimeFrame.Hour));
                var qqqbars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("QQQ", from, into, BarTimeFrame.Hour));
                var vtibars = await DClient.ListHistoricalBarsAsync(new HistoricalBarsRequest("VTI", from, into, BarTimeFrame.Hour));

                var bars = new List<StonkValues>();
                var aapl = new List<StonkValues>();
                
                foreach(var b in aaplbars.Items.Take(r.Next(2, aaplbars.Items.Count())))
                {
                    bars.Add(GetValues(b));
                    aapl.Add(GetValues(b));
                }
                foreach(var b in qqqbars.Items.Take(r.Next(2, qqqbars.Items.Count())))
                {
                    bars.Add(GetValues(b));
                }
                foreach(var b in vtibars.Items.Take(r.Next(2, vtibars.Items.Count())))
                {
                    bars.Add(GetValues(b));
                }
                
                var testmax = StonkValues.GetMax(aapl, true);
                var testmin = StonkValues.GetMax(aapl, false);

                net.Propogate(bars, testmax, testmin, new WriteToCMDLine(Write));


            }
            catch (Exception e)
            {
                e.OutputError();
            }
            
            if (clock.IsOpen)
            {
                var AAPLQuote = GetValues(await DClient.GetLatestQuoteAsync("AAPL"));
                var QQQQuote = GetValues(await DClient.GetLatestQuoteAsync("QQQ"));
                var VTIQuote = GetValues(await DClient.GetLatestQuoteAsync("VTI"));
                
                vals.Add(AAPLQuote);
                vals.Add(QQQQuote);
                vals.Add(VTIQuote);
                
                try
                {
                    if(vals.Count() > 6)
                    {
                        if(vals.Any(x => x.Symbol == "AAPL"))
                        {
                            var prediction = net.Predict(vals, new WriteToCMDLine(Write));
                            Write("Prediction : " + prediction[0] + ", " + prediction[1]);
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
