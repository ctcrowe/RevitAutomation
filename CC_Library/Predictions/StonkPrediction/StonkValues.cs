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
        public int[] indices { get; set; }
        public double[] change { get; set; }
        public double[] variability { get; set; }
        public double[] secondary { get; set; }
        public double[] hours { get; set; }
        public double[] volume { get; set; }
        public StonkValues()
        {
            this.indices = new int[1];
            this.change = new double[1];
            this.variability = new double[1];
            this.secondary = new double[1];
            this.hours = new double [1];
            this.volume = new double[1];
        }
        public StonkValues(int count)
        {
            this.indices = new int[count];
            this.change = new double[count];
            this.variability = new double[count];
            this.secondary = new double[count];
            this.hours = new double[count];
            this.volume = new double[count];
        }
        public double[] Locate(int numb)
        {
            double[] output = new double[8];
            output[indices[numb]] = 1;
            output[3] = change[numb];
            output[4] = variability[numb];
            output[5] = secondary[numb];
            output[6] = hours[numb];
            output[7] = volume[numb];

            return output;
        }
    }
}
