﻿using System;
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
}