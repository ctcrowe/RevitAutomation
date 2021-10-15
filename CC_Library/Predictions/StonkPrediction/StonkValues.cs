using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    /// <summary>
    /// index of stonk in enum [enum size]
    /// % change of stonk (price at close - price at open) / price at close
    /// variability ratio (high - low) / price at close
    /// secondary ratio (price at close - vwap / price at close
    /// hours since change
    /// Volume
    /// </summary>
    [Serializable]
    public class StonkValues
    {
        public Guid id { get; }
        public string Symbol { get; }
        public DateTime Time { get; }
        public double AskPrice { get; set; }
        public double BidPrice { get; set; }
        public StonkValues(string symbol, DateTime dt, double askprice, double bidprice)
        {
            this.id = new Guid();
            this.Symbol = symbol;
            this.Time = dt;
            this.AskPrice = askprice;
            this.BidPrice = bidprice;
        }
        public StonkValues(string symbol, DateTime dt, double price)
        {
            this.id = new Guid();
            this.Symbol = symbol;
            this.Time = dt;
            this.AskPrice = price;
            this.BidPrice = price;
        }
        public Comparison Coordinate(StonkValues v1)
        {
            Comparison Comp = new Comparison();
            Comp.Values[0] = Symbol == "AAPL" ? 1 : 0;
            Comp.Values[1] = Symbol == "QQQ" ? 1 : 0;
            Comp.Values[2] = Symbol == "VTI" ? 1 : 0;
            Comp.Values[3] = (this.Time - v1.Time).TotalHours;
            Comp.Values[4] = (v1.Time.Hour + (v1.Time.Minute / 60)) / 24.0;
            Comp.Values[6] = (this.AskPrice - v1.AskPrice) / v1.AskPrice;
            Comp.Values[7] = (this.BidPrice - v1.BidPrice) / v1.BidPrice;
            return Comp;
        }
        public static double[] GetMax(List<StonkValues> vals, bool minmax)
        {
            double[] output = new double[24];
            Dictionary<StonkValues, double> data = new Dictionary<StonkValues, double>();
            for(int j = 0; j < vals.Count(); j++)
            {
                data.Add(vals[j], vals[j].BidPrice + vals[j].AskPrice);
            }
            var valuetosearch = minmax ? data.Values.Max() : data.Values.Min();
            int time = data.Where(x => x.Value == valuetosearch).First().Key.Time.Hour;
            output[time] = 1;
            return output;
        }
        public void Save()
        {
            string folder = "StockValues".GetMyDocs();
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string subfolder = folder + "\\" + Symbol;
            if(!Directory.Exists(subfolder))
                Directory.CreateDirectory(subfolder);
            string fn = subfolder + "\\" + Symbol + "_" + Time.ToString("yyyyMMddhhmmss") + ".bin";
            fn.WriteToBinaryFile<StonkValues>(this);            
        }
    }
    public class Comparison
    {
        public double[] Values {get; set;}
        public Comparison()
        {
            this.Values = new double[8];
        }
        
        public static List<Comparison> GenerateComparisons(List<StonkValues> Vals)
        {
            List<Comparison> comps = new List<Comparison>();
            foreach(StonkValues val in Vals)
            {
                if (Vals.Any(x => x.Symbol == val.Symbol))
                {
                    var v = Vals.Where(x => x.Symbol == val.Symbol).ToList();
                    if(v.Any(y => DateTime.Compare(y.Time, val.Time) > 0))
                    {
                        var V1 = v.Where(z => DateTime.Compare(z.Time, val.Time) > 0).OrderBy(y => y.Time).First();
                        comps.Add(val.Coordinate(V1));
                    }
                }
            }
            return comps;
        }
    }
}
