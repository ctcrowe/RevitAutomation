using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal class Alpha
    {
        internal Alpha()
        {
            Network = Datatype.Alpha.LoadNetwork();
            if(Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.Alpha);
                Network.Layers.Add(new Layer(DictSize, ((2 * SearchRange) + 1) * CharSet.CharCount, Activation.LRelu));
                Network.Layers.Add(new Layer(DictSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(DictSize, Network.Layers.Last().Weights.GetLength(0), Activation.Linear));
            }
        }
        
        public const int DictSize = 30;
        public const int SearchRange = 3;
        public NeuralNetwork Network { get; }
        
        public double[] Forward(string s, AlphaContext context)
        {
            double[] ctxt = new double[s.Length];
            double[,] loc = new double[s.Length, DictSize];
            
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
            double[,] loc = new double[s.Length, DictSize];
            
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
                am.GlobalContextOutputs[j] = context.Contextualize(s, j, am);
            });
            return loc.Multiply(Activations.SoftMax(am.GlobalContextOutputs));
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
                    mem.Layers[i].DBiases(ldv, Network.Layers[i]);
                    mem.Layers[i].DWeights(ldv, am.LocationOutputs[j][i], Network.Layers[i]);
                    ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}
