using System;
using System.Linq;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    [Serializable]
    public class Layer
    {
        public double[,] Weights { get; set; }
        public double[] Biases { get; set; }
        public Activation Function { get; set; }

        #region Overloads
        public Layer(int NeuronCount, int WeightCount, Activation function)
        {
            Random random = new Random();
            this.Weights = new double[NeuronCount, WeightCount];
            this.Biases = new double[NeuronCount];
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
        public Layer(int NeuronCount, Layer PreviousLayer, Activation function)
        {
            Random random = new Random();
            this.Weights = new double[NeuronCount, PreviousLayer.Weights.GetLength(0)];
            this.Biases = new double[NeuronCount];
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
        public double[] Output(double[] Input)
        {
            double[] Output = new double[Weights.GetLength(0)];
            try
            {
                for (int i = 0; i < Output.Count(); i++)
                {
                    double result = 0;
                    for (int j = 0; j < Input.Count(); j++)
                    {
                        result += Input[j] * Weights[i, j];
                    }
                    result += Biases[i];
                    Output[i] = result;
                }
            }
            catch(Exception e) { e.OutputError(); }
            var func = Function.GetFunction();
            return func(Output);
        }
    }
}
