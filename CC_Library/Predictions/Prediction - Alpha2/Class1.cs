/*
    TODO:
    New Locate Command that uses a network to predict the beginning and end of each word. This will then advance the term by n, where n is the length of the word.
    Ultimately, breaking a phrase down into words. Search Radius will need to be substantially large, potentially 10 characters +/-. This will give us access to a set of pseudo words,
    without having to instantiate a dictionary for prediction purposes, giving more flexibility than a dictionary, but more structure than just letters to determine terms.
    
    This Network needs to be relatively small and quick, to interpret words on the fly fairly efficiently.
    Base Layer 1 size to have search radius 2 and Locate by character.
    Additional Base Layer to have coordintaed search size and locate a set of characters (potentially turns them into something like a syllable.)
    These syllables will then be interpreted into words, starting and ending being highlighted.
*/
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal class Alpha
    {
        internal Alpha(WriteToCMDLine write)
        {
            Network = Datatype.Alpha.LoadNetwork(write);
            if(Network.Datatype == Datatype.None)
            {
                Network = new NeuralNetwork(Datatype.Alpha);
                Network.Layers.Add(new Layer(DictSize, ((2 * SearchRange) + 1) * CharSet.CharCount, Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(DictSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(DictSize, Network.Layers.Last().Weights.GetLength(0), Activation.Linear, 1e-5, 1e-5));
            }
        }
        
        public const int DictSize = 40;
        public const int SearchRange = 3;
        public NeuralNetwork Network { get; }
        
        public double[] Forward(string s, AlphaContext context)
        {
            double[] ctxt = new double[s.Length];
            double[,] loc = new double[s.Length, DictSize];
            /*
            Parallel.For(0, s.Length, j =>
            {
                double[] a = s.Locate(j, SearchRange);
                for (int i = 0; i < Network.Layers.Count(); i++) { a = Network.Layers[i].Output(a); }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(s, j);
            });
            */
            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public double[] Forward(string s, AlphaContext context, AlphaMem am)
        {
            double[,] loc = new double[s.Length, DictSize];
            
            Parallel.For(0, s.Length, j =>
            {
                double[] a = s.Locate(j, SearchRange);
                //am.LocationOutputs[j].Add(a);
                for (int i = 0; i < Network.Layers.Count(); i++)
                {
                    //a = Network.Layers[i].Output(a);
                    //am.LocationOutputs[j].Add(a);
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
                    //ldv = mem.Layers[i].DActivation(ldv, am.LocationOutputs[j][i + 1]);
                    //mem.Layers[i].DBiases(ldv, Network.Layers[i], s.Length);
                    //mem.Layers[i].DWeights(ldv, am.LocationOutputs[j][i], Network.Layers[i], s.Length);
                    //ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}