using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions.StonkPrediction
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
        public int[] indices { get; set; }
        public double[] change { get; set; }
        public double[] variability { get; set }
        public double[] secondary { get; set; }
        public double[] hours { get; set; }
        public double[] volume { get; set; }
    }
    public StonkValues()
    {
        this.indices = new int[1] {0};
        this.change = new double[1] {0};
        this.variability = new double[1] {0};
        this.secondary = new double[1] {0};
        this.hours = new double[1] {0};
        this.volume = new double[1];
    }
    public StonkValues(int ct)
    {
        this.indices = new int[ct];
        this.change = new double[ct];
        this.variability = new double[ct];
        this.secondary = new double[ct];
        this.hours = new double[ct];
        this.volume = new double[ct];
    }
}
