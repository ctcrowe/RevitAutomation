using System;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    [Serializable]
    public class Layer
    {
        public double[,] Weights { get; set; }
        public double[] Biases { get; set; }
        public double[,] WMomentum { get; set; }
        public double[] BMomentum { get; set; }
        public double L1Regularization { get; set; }
        public double L2Regularizattion { get; set; }
        public Activation Function { get; set; }

        #region Overloads
        public Layer(int NeuronCount, int WeightCount, Activation function, double L1R = 0, double L2R = 0)
        {
            Random random = new Random();
            this.Weights = new double[NeuronCount, WeightCount];
            this.Biases = new double[NeuronCount];
            this.WMomentum = new double[NeuronCount, WeightCount];
            this.BMomentum = new double[NeuronCount];
            this.L1Regularization = L1R;
            this.L2Regularizattion = L2R;
            this.Function = function;
            for (int i = 0; i < NeuronCount; i++)
            {
                for (int j = 0; j < WeightCount; j++)
                {
                    this.Weights[i, j] = random.NextDouble() > 0.5 ? 
                        random.NextDouble() : (-1 * random.NextDouble());
                }
            }
        }
        public Layer(int NeuronCount, Layer PreviousLayer, Activation function, double L1R = 0, double L2R = 0)
        {
            Random random = new Random();
            this.Weights = new double[NeuronCount, PreviousLayer.Weights.GetLength(0)];
            this.Biases = new double[NeuronCount];
            this.WMomentum = new double[NeuronCount, PreviousLayer.Weights.GetLength(0)];
            this.BMomentum = new double[NeuronCount];
            this.L1Regularization = L1R;
            this.L2Regularizattion = L2R;
            this.Function = function;
            for (int i = 0; i < NeuronCount; i++)
            {
                for (int j = 0; j < PreviousLayer.Weights.GetLength(0); j++)
                {
                    this.Weights[i, j] = random.NextDouble() > 0.5 ? 
                        random.NextDouble() : (-1 * random.NextDouble());
                }
            }
        }
        #endregion
        public double[,] Output(double[] Input, double dropout = 0.1)
        {
            double[,] output = new double[2, Weights.GetLength(0)];
            double[] forward = new double[Weights.GetLength(0)];
            try
            {
                for (int i = 0; i < forward.Count(); i++)
                {
                    double result = 0;
                    for (int j = 0; j < Input.Count(); j++)
                    {
                        result += Input[j] * Weights[i, j];
                    }
                    result += Biases[i];
                    forward[i] = result;
                }
            }
            catch(Exception e) { e.OutputError(); }
            
            var func = Function.GetFunction();
            output.SetRank(func(output), 0);
            output.SetRank(Dropout(output.GetRank(0), dropout), 1);
            return output;
        }
        public void Update()
        {
            Parallel.For(0, Weights.GetLength(0), j =>
                         { Parallel.For(0, Weights.GetLength(1), i => 
                                        {
                                            Weights[j, i] += WMomentum[j, i];
                                            WMomentum[j, i] *= 0.5;
                                        });
                         });
            Parallel.For(0, Biases.Count(), j =>
                         {
                             Biases[j] += BMomentum[j];
                             BMomentum[j] *= 0.5;
                         });
            
        }
        private static double[] DropOut(double[] input, double rate)
        {
            double[] output = new double[input.Count()];
            var DOLayer = input.RandomBinomial(rate);
            for(int i = 0; i < output.Count(); i++)
            {
                output[i] = input[i] * DOLayer[i] / (1 - rate);
            }
            return output;
        }
    }
}
