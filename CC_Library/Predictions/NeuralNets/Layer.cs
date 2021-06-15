using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    [Serializable]
    public class Layer
    {
        public double[,] Weights { get; set; }
        public double[,] DeltaW { get; set; }
        public double[] Biases { get; set; }
        public double[] DeltaB { get; set; }
        public int Number { get; set; }
        public Activation Function { get; set; }

        #region Overloads
        public Layer(int NeuronCount, int WeightCount, Activation function)
        {
            Random random = new Random();
            this.Weights = new double[NeuronCount, WeightCount];
            this.Biases = new double[NeuronCount];
            this.DeltaW = new double[NeuronCount, WeightCount];
            this.DeltaB = new double[NeuronCount];
            this.Number = 0;
            this.Function = function;
            for (int i = 0; i < NeuronCount; i++)
            {
                for (int j = 0; j < WeightCount; j++)
                {
                    this.Weights[i, j] = random.NextDouble();
                    if (random.NextDouble() < 0.5)
                        this.Weights[i, j] *= -1;
                }
            }
        }
        public Layer(int NeuronCount, Layer PreviousLayer, Activation function)
        {
            Random random = new Random();
            this.Weights = new double[NeuronCount, PreviousLayer.Weights.GetLength(0)];
            this.Biases = new double[NeuronCount];
            this.DeltaW = new double[NeuronCount, PreviousLayer.Weights.GetLength(0)];
            this.DeltaB = new double[NeuronCount];
            this.Number = PreviousLayer.Number + 1;
            this.Function = function;
            for (int i = 0; i < NeuronCount; i++)
            {
                for (int j = 0; j < PreviousLayer.Weights.GetLength(0); j++)
                {
                    this.Weights[i, j] = random.NextDouble();
                    if (random.NextDouble() < 0.5)
                        this.Weights[i, j] *= -1;
                }
            }
        }
        public Layer(XElement ele)
        {
            int Neurons = ele.Elements("Neuron").Count();
            int Weight = ele.Elements("Neuron").First().Elements("Weight").Count();
            this.Number = int.Parse(ele.Attribute("Number").Value);
            this.Function = (Activation)Enum.Parse(typeof(Activation), ele.Attribute("Function").Value);

            this.Biases = new double[Neurons];
            this.Weights = new double[Neurons, Weight];
            this.DeltaW = new double[Neurons, Weight];
            this.DeltaB = new double[Neurons];

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
            if(Input.Count() != Weights.GetLength(1))
            {
                double[,] NewWeights = new double[Output.Count(), Input.Count()];
                double[] NewBiases = new double[Output.Count()];
                if (Input.Count() < Weights.GetLength(1))
                {
                    for (int i = 0; i < Output.Count(); i++)
                    {
                        NewBiases[i] = Biases[i];
                        for (int j = 0; j < Input.Count(); j++)
                        {
                            NewWeights[i, j] = Weights[i, j];
                        }
                    }
                    Weights = NewWeights;
                    Biases = NewBiases;
                }
                else
                {
                    for(int i = 0; i < Output.Count(); i++)
                    {
                        NewBiases[i] = Biases[i];
                        for (int j = 0; j < Weights.GetLength(1); j++)
                        {
                            NewWeights[i, j] = Weights[i, j];
                        }
                    }
                }
            }
            if (Input.Count() == Weights.GetLength(1))
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
            var func = Function.GetFunction();
            return func(Output);
        }
        internal void Update(double adjustment, LayerMem lm)
        {
            for (int i = 0; i < DeltaB.Count(); i++)
            {
                if (lm.DeltaB[i] == double.PositiveInfinity || lm.DeltaB[i] == double.NegativeInfinity)
                {
                    if (lm.DeltaB[i] == double.PositiveInfinity)
                        this.Biases[i] -= adjustment;
                    else
                        this.Biases[i] += adjustment;
                }
                else
                {
                    if (!double.IsNaN(lm.DeltaB[i]))
                        this.Biases[i] -= (adjustment * lm.DeltaB[i]);
                }
            }
            for (int i = 0; i < Weights.GetLength(0); i++)
            {
                for (int j = 0; j < Weights.GetLength(1); j++)
                {
                    if (lm.DeltaW[i, j] == double.PositiveInfinity || lm.DeltaW[i, j] == double.NegativeInfinity)
                    {
                        if (lm.DeltaW[i, j] == double.PositiveInfinity)
                            lm.DeltaW[i, j] -= adjustment;
                        else
                            lm.DeltaW[i, j] += adjustment;
                    }
                    else
                    {
                        if (!double.IsNaN(lm.DeltaW[i, j]))
                            this.Weights[i, j] -= (adjustment * lm.DeltaW[i, j]);
                    }
                }
            }
            lm.DeltaW = new double[Weights.GetLength(0), Weights.GetLength(1)];
            lm.DeltaB = new double[Biases.Count()];
        }
        public void Update(double adjustment)
        {
            for (int i = 0; i < DeltaB.Count(); i++)
            {
                if (DeltaB[i] == double.PositiveInfinity || DeltaB[i] == double.NegativeInfinity)
                {
                    if (DeltaB[i] == double.PositiveInfinity)
                        this.Biases[i] -= adjustment;
                    else
                        this.Biases[i] += adjustment;
                }
                else
                {
                    if (!double.IsNaN(DeltaB[i]))
                        this.Biases[i] -= (adjustment * DeltaB[i]);
                }
            }
            for (int i = 0; i < Weights.GetLength(0); i++)
            {
                for (int j = 0; j < Weights.GetLength(1); j++)
                {
                    if (DeltaW[i, j] == double.PositiveInfinity || DeltaW[i, j] == double.NegativeInfinity)
                    {
                        if (DeltaW[i, j] == double.PositiveInfinity)
                            DeltaW[i, j] -= adjustment;
                        else
                            DeltaW[i, j] += adjustment;
                    }
                    else
                    {
                        if (!double.IsNaN(DeltaW[i, j]))
                            this.Weights[i, j] -= (adjustment * DeltaW[i, j]);
                    }
                }
            }
            this.DeltaW = new double[Weights.GetLength(0), Weights.GetLength(1)];
            this.DeltaB = new double[Biases.Count()];
        }

        #region CostFunctions
        //DActivation ALWAYS needs to equal number of nodes = number of Biases
        public double[] DActivation(double[] dvalues, double[] output)
        {
            var function = Function.InvertFunction();
            var deriv = function(dvalues, output);
            return deriv;
        }
        public void DBiases(double[] dvalues)
        {
            DeltaB.Add(dvalues);
        }
        public void DWeights(double[] dvalues, double[] inputs)
        {
            for(int i = 0; i < DeltaW.GetLength(0); i++)
            {
                for(int j = 0; j < DeltaW.GetLength(1); j++)
                {
                    DeltaW[i, j] += inputs[j] * dvalues[i];
                }
            }
        }
        public double[] DInputs(double[] dvalues)
        {
            double[] result = new double[Weights.GetLength(1)];
            for(int i = 0; i < this.Weights.GetLength(0); i++)
            {
                for(int j = 0; j < this.Weights.GetLength(1); j++)
                {
                    result[j] += dvalues[i] * this.Weights[i, j];
                }
            }
            return result;
        }
        #endregion
    }
}