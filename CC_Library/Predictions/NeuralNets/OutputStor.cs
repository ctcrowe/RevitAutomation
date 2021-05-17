using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal class OutputStor
    {
        public double[] Output { get; set; }
        public double[] Input { get; set; }
        public OutputStor(Layer l)
        {
            Output = new double[l.Biases.Count()];
            Input = new double[l.Weights.GetLength(1)];
        }
    }
}
