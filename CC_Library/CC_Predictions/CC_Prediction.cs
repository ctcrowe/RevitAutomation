using System.Collections.Generic;
using System;
using System.Linq;

namespace CC_Library
{

    internal class Prediction
    {
        public string Name { get; set; }
        public double Value { get; set; }
        private int Count { get; set; }

        public Prediction(PredictionOption o, PredictionElement e)
        {
            this.Name = o.Name;
            this.Count = 1;
            this.Value = o.CalcAdjustment(e);
        }
        public void Combine(PredictionElement e)
        {
            if(e.Options.Any(x => x.Name == Name))
            {
                double v = this.Value * this.Count;
                double adjust = e.Options.Where(x => x.Name == Name).First().CalcAdjustment(e);
                this.Count += 1;
                double total = adjust + v;
                this.Value = total / Count;
            }
        }
    }
}
