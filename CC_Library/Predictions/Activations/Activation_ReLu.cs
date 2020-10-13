using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    class Activation_ReLu
    {
        public List<double[]> inputs;
        public List<double[]> outputs;
        public List<double[]> dinputs;

        public Activation_ReLu()
        {
            this.inputs = new List<double[]>();
            this.outputs = new List<double[]>();
            this.dinputs = new List<double[]>();
        }

        public double[] Forward(double[] input)
        {
            this.inputs.Add(input);
            double[] output = new double[input.Count()];
            for(int i = 0; i < input.Count(); i++)
            {
                output[i] = Math.Max(0, input[i]);
            }
            return output;
        }
        public List<double[]> Backward(List<double[]> dvalues)
        {
            if (dvalues.Count() == outputs.Count())
            {
                for(int i = 0; i < dvalues.Count(); i++)
                {
                    double[] dinput = new double[dvalues[i].Count()];
                    for(int j = 0; j < this.inputs[i].Count(); j++)
                    {
                        if (inputs[i][j] <= 0) dinput[j] = 0;
                        else dinput[j] = dvalues[i][j];
                    }
                    dinputs.Add(dinput);
                }
            }
            return dinputs;
        }
    }
}
