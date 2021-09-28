using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal class Stonk
    {
        /// <summary>
        /// index of stonk in enum [enum size]
        /// % change of stonk (price at close - price at open) / price at close
        /// variability ratio (high - low) / price at close
        /// secondary ratio (price at close - vwap / price at close
        /// hours since change
        /// Volume
        /// </summary>
        internal Stonk(double[,] vals)
        {
            Network = Datatype.Stonk.LoadNetwork();
            if (Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.Stonk);
                Network.Layers.Add(new Layer(MktSize, 8, Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.Linear));
            }
        }

        public const int MktSize = 30;
        public NeuralNetwork Network { get; }
        
        public static double[] Coordinate(StonkValues v1,StonkValues v2)
        {
            double[] vals = new double[5];
            vals[0] = DateTime.TotalHours(v1, v2);
            vals[1] = (v2.AskPrice - v1.AskPrice) / v1.AskPrice;
            vals[2] = (v2.AskSize - v1.AskSize) / v1.AskSize;
            vals[3] = (v2.BidPrice - v1.BidPrice) / v1.BidPrice;
            vals[4] = (v2.BidSize - v1.BidSize) / v1.BidSize;
        }

        public double[] Forward(List<StonkValues> vals, StonkContext context)
        {
            double[] ctxt = new double[vals.Count()];
            double[,] loc = new double[vals.Count(), MktSize];
            var newvals = vals.OrderBy(x => x.Time);

            Parallel.For(0, vals.Count() - 1, j =>
            {
                double[] a = vals[j].Coordinate(vals[j + 1]);
                for (int i = 0; i < Network.Layers.Count(); i++) { a = Network.Layers[i].Output(a); }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(vals, j);
            });

            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public double[] Forward(double[,] vals, StonkContext context, StonkMem sm)
        {
            double[] ctxt = new double[vals.GetLength(0)];
            double[,] loc = new double[vals.GetLength(0), MktSize];

            Parallel.For(0, vals.GetLength(0), j =>
            {
                double[] a = vals.GetRank(j);
                sm.LocationOutputs[j].Add(a);
                for (int i = 0; i < Network.Layers.Count(); i++)
                {
                    a = Network.Layers[i].Output(a);
                    sm.LocationOutputs[j].Add(a);
                }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(vals, j, sm);
            });
            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public void Backward(string s, double[] DValues, StonkContext context, StonkMem sm, NetworkMem mem, NetworkMem CtxtMem)
        {
            var LocDValues = sm.DLocation(DValues);
            DValues = sm.DGlobalContext(DValues);
            DValues = Activations.InverseSoftMax(DValues, sm.GlobalOutputs);
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
