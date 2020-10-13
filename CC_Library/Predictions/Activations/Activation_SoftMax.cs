using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal class Activation_SoftMax
    {
        public List<double[]> inputs;
        public List<double[]> outputs;
        public List<double[]> dinputs;

        public Activation_SoftMax()
        {
            this.inputs = new List<double[]>();
            this.outputs = new List<double[]>();
            this.dinputs = new List<double[]>();
        }

        public double[] Forward(double[] input)
        {
            this.inputs.Add(input);
            double[] ExpVals = new double[input.Count()];
            double[] output = new double[input.Count()];
            for(int i = 0; i < ExpVals.Count(); i++)
            {
                ExpVals[i] = Math.Exp(input[i] - input.Max());
            }
            for(int i = 0; i < ExpVals.Count(); i++)
            {
                output[i] = ExpVals[i] / ExpVals.Sum();
            }
            outputs.Add(output);
            return output;
        }
        public List<double[]> Backward(List<double[]> dvalues)
        {
            if(dvalues.Count() == outputs.Count())
            {
                for(int i = 0; i < dvalues.Count(); i++)
                {
                    double[] result = new double[dvalues[i].Count()];
                    double[,] diag = outputs[i].DiagFlat();
                    double[,] Jacobian = new double[diag.GetLength(0), diag.GetLength(1)];

                    for(int j = 0; j < Jacobian.GetLength(0); j++)
                    {
                        for(int k = 0; k < Jacobian.GetLength(1); k++)
                        {
                            Jacobian[j, k] = diag[j, k] - (outputs[i][j] * outputs[i][k]);
                            result[j] += Jacobian[j, k] * dvalues[i][k];
                        }
                    }
                    dinputs.Add(result);
                }
            }
            return dinputs;
        }
    }
}
