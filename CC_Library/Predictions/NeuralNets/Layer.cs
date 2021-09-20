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
        public Layer(XElement ele)
        {
            int Neurons = ele.Elements("Neuron").Count();
            int Weight = ele.Elements("Neuron").First().Elements("Weight").Count();
            this.Function = (Activation)Enum.Parse(typeof(Activation), ele.Attribute("Function").Value);

            this.Biases = new double[Neurons];
            this.Weights = new double[Neurons, Weight];

            for (int i = 0; i < Neurons; i++)
            {
                XElement n = ele.Elements("Neuron").Where(x => x.Attribute("Location").Value == i.ToString()).First();
                this.Biases[i] = double.Parse(n.Attribute("Bias").Value);
                for (int j = 0; j < Weight; j++)
                {
                    this.Weights[i, j] = double.Parse(n.Elements("Weight")
                        .Where(x => x.Attribute("Number").Value == j.ToString()).First()
                        .Attribute("Value").Value);
                }
            }
        }
        #endregion
        //Input must be equal to number of Weights, Output equal to number of nodes / biases.
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
