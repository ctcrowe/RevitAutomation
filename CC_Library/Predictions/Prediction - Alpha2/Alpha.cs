using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Collections.Generic;

namespace CC_Library.Predictions
{
    [Serializable]
    internal class Alpha2
    {
        private List<IAlphaFilter> Filters { get; }
        internal Alpha2(WriteToCMDLine write)
        {
            this.Filters = new List<IAlphaFilter>();
            Filters.Add(new AlphaFilter1(write));
            Filters.Add(new AlphaFilter2(write));
            Filters.Add(new AlphaFilter3(write));
            Filters.Add(new AlphaFilter4(write));
        }
        public int GetSize()
        {
            int size = 0;
            for(int i = 0; i < Filters.Count; i++)
            {
                size += Filters[i].GetSize();
            }
            return size;
        }
        public AlphaMem[] CreateAlphaMemory(string s)
        {
            AlphaMem[] mem = new AlphaMem[Filters.Count];
            Parallel.For(0, Filters.Count, j => mem[j] = new AlphaMem(s.ToCharArray()));
            return mem;
        }
        public NetworkMem[,] CreateMemory()
        {
            NetworkMem[,] mem = new NetworkMem[Filters.Count(), 2];
            try
            {
                Parallel.For(0, Filters.Count, j =>
                             {
                                 mem[j, 0] = new NetworkMem(Filters[j].ValueNetwork);
                                 mem[j, 1] = new NetworkMem(Filters[j].AttentionNetwork);
                             });
            }
            catch (Exception e) { e.OutputError(); }
            return mem;
        }
        public double[] Forward(string s, AlphaMem[] am, WriteToCMDLine write)
        {
            try
            {
                if (am.Length == Filters.Count())
                {
                    List<double> output = new List<double>();
                    for (int i = 0; i < Filters.Count(); i++)
                    {
                        output.AddRange(Filters[i].Forward(s, am[i]));
                    }
                    return output.ToArray();
                }
                else
                    return null;
            }
            catch (Exception e) { e.OutputError(); }
            return null;
        }
        public void Backward(string s, double[] DValues, AlphaMem[] am, NetworkMem[,] mem, WriteToCMDLine write)
        {
            try
            {
                int start = 0;
                for(int i = 0; i < Filters.Count(); i++)
                {
                    var size = Filters[i].GetSize();
                    var dvals = DValues.ToList().GetRange(start, size).ToArray();
                    Filters[i].Backward(s, dvals, am[i], mem[i, 0], mem[i, 1]);
                    start += size;
                }
            }
            catch (Exception e) { e.OutputError(); }
        }
        public void Update(NetworkMem[,] mem, int runsize = 16)
        {
            try
            {
                Parallel.For(0, Filters.Count, j =>
                {
                    mem[j, 0].Update(runsize, Filters[j].GetChangeSize(), Filters[j].ValueNetwork);
                    mem[j, 1].Update(runsize, Filters[j].GetChangeSize(), Filters[j].AttentionNetwork);
                });
            }
            catch (Exception e) { e.OutputError(); }
        }
    }
}
