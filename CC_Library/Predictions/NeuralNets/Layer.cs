using System;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    [Serializable]
    public class Layer
    {
        public int LayerNumb {get; set;}
        public double[,] Weights { get; set; }
        public double[] Biases { get; set; }
        public double[,] WMomentum { get; set; }
        public double[] BMomentum { get; set; }
        public double L1Regularization { get; set; }
        public double L2Regularizattion { get; set; }
        public Activation Function { get; set; }

        #region Overloads
        public Layer(int Outputs, int Inputs, Activation function, double L1R = 0, double L2R = 0)
        {
            Random random = new Random();
            this.LayerNumb = 0;
            this.Weights = new double[Outputs, Inputs];
            this.Biases = new double[Outputs];
            this.WMomentum = new double[Outputs, Inputs];
            this.BMomentum = new double[Outputs];
            this.L1Regularization = L1R;
            this.L2Regularizattion = L2R;
            this.Function = function;
            for (int i = 0; i < NeuronCount; i++)
            {
                for (int j = 0; j < WeightCount; j++)
                {
                    this.Weights[i, j] = random.NextDouble() > 0.5 ? 
                        random.NextDouble() / Inputs : (-1 * random.NextDouble() / Inputs);
                }
            }
        }
        public Layer(int NeuronCount, Layer PreviousLayer, Activation function, double L1R = 0, double L2R = 0)
        {
            Random random = new Random();
            this.LayerNumb = PreviousLayer.LayerNumb + 1;
            this.Weights = new double[NeuronCount, PreviousLayer.Weights.GetLength(0)];
            this.Biases = new double[NeuronCount];
            this.WMomentum = new double[NeuronCount, PreviousLayer.Weights.GetLength(0)];
            this.BMomentum = new double[NeuronCount];
            this.L1Regularization = L1R;
            this.L2Regularizattion = L2R;
            this.Function = function;
            
            var WeightCount = PreviousLayer.Weights.GetLength(0);
            for (int i = 0; i < NeuronCount; i++)
            {
                for (int j = 0; j < WeightCount; j++)
                {
                    this.Weights[i, j] = random.NextDouble() > 0.5 ? 
                        random.NextDouble() / WeightCount : (-1 * random.NextDouble() / WeightCount);
                }
            }
        }
        #endregion
            
        public double[] Output(double[] Input)
        {
            //if(Input.Any(x => double.IsNaN(x))) { throw new Exception("Inputs are NaN Values at Layer Number " + LayerNumb); }
            //if(Input.Any(x => x == null)) { throw new Exception("Inputs are null at Layer Number " + LayerNumb); }
            
            var result = Weights.Dot(Input).Add(Biases);
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
        public double[] DropOut(double[] input, double rate)
        {
            double[] output = new double[input.Count()];
            if (Function != Activation.SoftMax &&
                Function != Activation.CombinedCrossEntropySoftmax)
            {
                var DOLayer = input.RandomBinomial(rate);
                Parallel.For(0, output.Count(), i =>
                {
                    /*
                    output[i] = Function != Activation.Tangential &&
                        Function != Activation.Sigmoid ?
                        input[i] * DOLayer[i] / (1 - rate) :
                    */
                       output[i] = input[i] * DOLayer[i];
                });
            }
            return output;
        }
        public double[] InverseDropOut(double[] DValues, double[] DropOutRank)
        {
            double[] output = new double[DValues.Count()];
            Parallel.For(0, DValues.Count(), i =>
            {
                output[i] = Function != Activation.CombinedCrossEntropySoftmax &&
                    Function != Activation.SoftMax &&
                    DropOutRank[i] == 0 ? 0 : DValues[i];
            });
            return output;
        }
    }
}
