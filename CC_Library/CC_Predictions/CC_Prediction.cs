using System.Collections.Generic;
using System;

namespace CC_Library
{

    internal class Prediction
    {
        public string Name { get; set; }
        public double Value { get; set; }
        private int Count { get; set; }

        public Prediction( PredictionOption o)
        {
            this.Name = o.Name;
            this.Count = 1;
            this.Value = o.CalcAdjustment();
        }
        public void Combine(PredictionOption o)
        {
            double v = this.Value * this.Count;
            double adjust = o.CalcAdjustment();
            this.Count += 1;
            double total = adjust + v;
            this.Value = total / Count;
        }
    }
}
