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

        /*
        public List<double[,]> Forward(double[] input, double dropout, WriteToCMDLine write)
        {
            List<double[,]> Results = new List<double[,]>();
            double[,] resultinput = new double[2,input.Count()];
            resultinput.SetRank(input, 0);
            resultinput.SetRank(input, 1);
            Results.Add(resultinput);
            for (int k = 0; k < Layers.Count(); k++)
            {
                double[,] output = new double[2, Layers[k].Biases.Count()];
                var rank = Layers[k].Output(Results.Last().GetRank(1));
                if(rank.Any(x => double.IsNaN(x)))
                {
                    write("Layer " + k + " in " + Datatype.ToString() + " Network  has NaN Values");
                }
                output.SetRank(Layers[k].Output(Results.Last().GetRank(1)), 0);
                var drop = DropOut(output.GetRank(0), dropout, write);
                output.SetRank(drop, 1);
                Results.Add(output);
            }
            return Results;
        }
         */
        public double[] Output(double[] Input)
        {
            double[] result = new double[Biases.Length];
            Parallel.For(0, result.Count(), i =>
            {
                Parallel.For(0, Input.Count(), j => result[i] += Input[j] * Weights[i, j]);
                result[i] += Biases[i];
            });
            var func = Function.GetFunction();
            return func(result);
        }
        public double[,] Forward(double[] Input, double dropout = 0.1)
        {
            double[,] Output = new double[2, Biases.Count()];
            double[] result = new double[Biases.Count()];
            try
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    for (int j = 0; j < Input.Count(); j++)
                    {
                        result[i] += Input[j] * Weights[i, j];
                    }
                    result[i] += Biases[i];
                }
            }
            catch (Exception e) { e.OutputError(); }
            var func = Function.GetFunction();
            Output.SetRank(func(result), 0);
            Output.SetRank(DropOut(Output.GetRank(0), dropout), 1);
            return Output;
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
        public double[] InverseDropOut(double[] DValues, double[] DropOutRank)
        {
            double[] output = new double[DValues.Count()];
            for(int i = 0; i < DValues.Count(); i++)
            {
                output[i] = DropOutRank[i] == 0 ? 0 : DValues[i];
            }
            return output;
        }
    }
}
