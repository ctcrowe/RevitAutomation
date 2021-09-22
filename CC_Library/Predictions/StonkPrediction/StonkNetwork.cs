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
        internal Stonk()
        {
            Network = Datatype.Stonk.LoadNetwork();
            if (Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.Stonk);
                Network.Layers.Add(new Layer(MktSize, ((2 * SearchRange) + 1) * 6, Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.Linear));
            }
        }

        public const int MktSize = 30;
        public const int SearchRange = 3;
        public NeuralNetwork Network { get; }

        public double[] Forward(string s, AlphaContext context)
        {
            double[] ctxt = new double[s.Length];
            double[,] loc = new double[s.Length, MktSize];

            Parallel.For(0, s.Length, j =>
            {
                double[] a = s.Locate(j, SearchRange);
                for (int i = 0; i < Network.Layers.Count(); i++) { a = Network.Layers[i].Output(a); }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(s, j);
            });

            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public double[] Forward(string s, AlphaContext context, AlphaMem am)
        {
            double[] ctxt = new double[s.Length];
            double[,] loc = new double[s.Length, MktSize];

            Parallel.For(0, s.Length, j =>
            {
                double[] a = s.Locate(j, SearchRange);
                am.LocationOutputs[j].Add(a);
                for (int i = 0; i < Network.Layers.Count(); i++)
                {
                    a = Network.Layers[i].Output(a);
                    am.LocationOutputs[j].Add(a);
                }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(s, j, am);
            });
            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public void Backward(string s, double[] DValues, AlphaContext context, AlphaMem am, NetworkMem mem, NetworkMem CtxtMem)
        {
            var LocDValues = am.DLocation(DValues);
            DValues = am.DGlobalContext(DValues);
            DValues = Activations.InverseSoftMax(DValues, am.GlobalContextOutputs);
            context.Backward(DValues, s.Length, am, CtxtMem);
            Parallel.For(0, s.Length, j =>
            {
                var ldv = LocDValues[j];
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    ldv = mem.Layers[i].DActivation(ldv, am.LocationOutputs[j][i + 1]);
                    mem.Layers[i].DBiases(ldv);
                    mem.Layers[i].DWeights(ldv, am.LocationOutputs[j][i]);
                    ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}
