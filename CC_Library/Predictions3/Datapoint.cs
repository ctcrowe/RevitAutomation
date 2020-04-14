using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal class DataPoint
    {
        private string Label { get; }
        private double[] Data { get; set; }

        public DataPoint(string label)
        {
            this.Label = label;
            this.Data = new double[5];
        }
        public DataPoint(string label, double[] data)
        {
            this.Label = label;
            this.Data = data;
        }
    }
}
