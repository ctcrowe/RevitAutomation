using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public class Stonk
    {
        /// <summary>
        /// index of stonk in enum [enum size]
        /// % change of stonk (price at close - price at open) / price at close
        /// variability ratio (high - low) / price at close
        /// secondary ratio (price at close - vwap / price at close
        /// hours since change
        /// Volume
        /// </summary>
        public Stonk()
        {
            Network = Datatype.Stonk.LoadNetwork();
            if (Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.Stonk);
                Network.Layers.Add(new Layer(MktSize, 5, Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.Linear));
            }
        }

        public const int MktSize = 30;
        public NeuralNetwork Network { get; }

        public double[] Forward(List<StonkValues> vals, StonkContext context)
        {
            double[] ctxt = new double[vals.Count()];
            double[,] loc = new double[vals.Count(), MktSize];
            var newvals = vals.OrderBy(x => x.Time);

            for(int j = 0; j < vals.Count() - 1; j++)
            {
                double[] a = vals[j].Coordinate(vals[j + 1]);
                for (int i = 0; i < Network.Layers.Count(); i++) { a = Network.Layers[i].Output(a); }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(vals, j);
            }

            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public Sample Forward(List<StonkValues> vals, StonkContext ctxt, StonkMem sm)
        {
            Parallel.For(0, vals.Count() - 1, j =>
            {
                List<double[]> Location = new List<double[]>();
                Location.Add(vals[j].Coordinate(vals[j + 1]));
                for(int i = 0; i < Network.Layers.Count(); i++)
                {
                    Location.Add(Network.Layers[i].Output(Location.Last()));
                }
                sm.LocationOutputs.Add(Location);
                ctxt.Contextualize(Location.First(), sm);
            }
            Sample s = new Sample(Datatype.AAPL);
            s.ValInput = sm.Multiply();
            return s;
        }
        public void Backward(string s, double[] DValues, StonkContext context, StonkMem sm, NetworkMem mem, NetworkMem CtxtMem)
        {
            var LocDValues = sm.DLocation(DValues);
            DValues = sm.DGlobalContext(DValues);
            DValues = Activations.InverseSoftMax(DValues, sm.GlobalOutputs.ToArray());
            context.Backward(DValues, s.Length, sm, CtxtMem);
            Parallel.For(0, s.Length, j =>
            {
                var ldv = LocDValues[j];
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    ldv = mem.Layers[i].DActivation(ldv, sm.LocationOutputs[j][i + 1]);
                    mem.Layers[i].DBiases(ldv);
                    mem.Layers[i].DWeights(ldv, sm.LocationOutputs[j][i]);
                    ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}
