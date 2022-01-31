using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Collections.Generic;

namespace CC_Library.Predictions
{
    internal class Alpha2
    {
        private List<IAlphaFilter> Filters { get; }
        internal Alpha2(WriteToCMDLine write)
        {
            this.Filters = new List<IAlphaFilter>();
            Filters.Add(new AlphaFilter1(write));
            Filters.Add(new AlphaFilter2(write));
            Filters.Add(new AlphaFilter3(write));
        }
        public int GetSize()
        {
            int size = 0;
            for(int i = 0; i < Filters.Length; i++)
            {
                size += Filters[i].GetSize();
            }
            return size;
        }
        public double[] Forward(string s, AlphaMem[] am)
        {
            if(am.Length == Filters.Count())
            {
                List<double> output = new List<double>();
                for(int i = 0; i < Filters.Count(); i++)
                {
                    output.AddRange(Filters[i].Forward(s, am[i]));
                }
                return output.ToArray();
            }
            else
                return null;
        }
        public void Backward(string s, double[] DValues, AlphaMem am[], NetworkMem mem, NetworkMem CtxtMem)
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
                    mem.Layers[i].DBiases(ldv, Network.Layers[i], s.Length);
                    mem.Layers[i].DWeights(ldv, am.LocationOutputs[j][i], Network.Layers[i], s.Length);
                    ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}
