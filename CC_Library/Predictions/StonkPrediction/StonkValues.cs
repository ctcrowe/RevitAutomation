using System;
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
        Guid id { get; }
        string Symbol { get; }
        DateTime Time { get; }
        double AskPrice { get; set; }
        double AskSize { get; set; }
        double BidPrice { get; set; }
        double BidSize { get; set; }
        public StonkValues(string symbol, DateTime dt, double askprice, double asksize, double bidprice, double bidsize)
        {
            this.id = new Guid();
            this.Symbol = symbol;
            this.Time = dt;
            this.AskPrice = askprice;
            this.AskSize = asksize;
            this.BidPrice = bidprice;
            this.BidSize = bidsize;
        }
        public void Save()
        {
            string folder = "StockValues".GetMyDocs();
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string subfolder = folder + "\\" + Symbol;
            if(!Directory.Exists(subfolder))
                Directory.CreateDirectory(subfolder);
            
            
        }
    }
}
