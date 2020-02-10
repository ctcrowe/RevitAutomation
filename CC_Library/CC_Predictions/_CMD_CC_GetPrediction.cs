using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library
{
    class CMD_CC_GetPrediction
    {
        public string Predict()
        {
            foreach(PredictionElement e in Elements)
            {
                foreach(PredictionOption o in e.Options)
                {
                    if(!this.Options.Any(x => x.Name == o.Name))
                    {
                        this.Options.Add(new Prediction(o.Name));
                    }
                    double val = o.Weight * o.Adjustment;
                    this.Options.Where(x => x.Name == o.Name).First().Adjust(val);
                }
            }
            double Max = 0;
            foreach(Prediction p in this.Options)
            {
                if(p.Weight > Max)
                    Max = p.Weight;
            }
            string s = Options.Where(x => x.Weight == Max).First().Name;
            this.Prediction = s;
            return s;
        }
    }
}
