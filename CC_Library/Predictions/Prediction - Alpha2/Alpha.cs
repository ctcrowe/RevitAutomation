using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Collections.Generic;
using System.IO;

namespace CC_Library.Predictions
{
    internal class Alpha2
    {
        private List<IAlphaFilter> Filters { get; }
        internal Alpha2(WriteToCMDLine write)
        {
            this.Filters = new List<IAlphaFilter>();
            Filters.Add(new AlphaFilter1(write));
            //Filters.Add(new WordFilter(write));
            Filters.Add(new WordFilter2(write));
        }
        public int GetSize()
        {
            int size = 0;
            for (int i = 0; i < Filters.Count; i++)
            {
                size += Filters[i].GetSize();
            }
            return size;
        }
        public void Save()
        {
            string Folder = "NeuralNets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            Parallel.For(0, this.Filters.Count(), i => this.Filters[i].Save(Folder));
        }
        public void Load(WriteToCMDLine write)
        {
            Parallel.For(0, this.Filters.Count(), i => this.Filters[i] = this.Filters[i].LoadAlpha(write));
        }
        public NetworkMem[][] CreateMemory()
        {
            //change mem from NetworkMem[,] to NetworkMem[Filters.Count()][];
            NetworkMem[][] mem = new NetworkMem[Filters.Count()][];
            try
            {
                Parallel.For(0, Filters.Count, j =>
                             {
                                 mem[j] = new NetworkMem[Filters[j].Networks.Count()];
                                 for (int i = 0; i < Filters[j].Networks.Count(); i++)
                                 {
                                     mem[j][i] = new NetworkMem(Filters[j].Networks[i]);
                                 }
                             });
            }
            catch (Exception e) { e.OutputError(); }
            return mem;
        }
        public KeyValuePair<double[], List<double[][][][][]>> Forward(string s, WriteToCMDLine write)
        {
            List<double[][][][][]> output = new List<double[][][][][]>();
            List<double> fin = new List<double>();
            try
            {
                for (int i = 0; i < Filters.Count(); i++)
                {
                    output.Add(Filters[i].Forward(s));
                    fin.AddRange(output.Last().Last().Last().Last().Last());
                }
            }
            catch (Exception e) { e.OutputError(); }
            return new KeyValuePair<double[], List<double[][][][][]>>(fin.ToArray(), output);
        }
        public void Backward(double[] DValues, List<double[][][][][]> outputs, NetworkMem[][] mem, WriteToCMDLine write, bool tf = false)
        {
            var start = 0;
            try
            {
                for (int i = 0; i < Filters.Count(); i++)
                {
                    var size = Filters[i].GetSize();
                    var dvals = DValues.ToList().GetRange(start, size).ToArray();
                    Filters[i].Backward(dvals, /*am[i]*/outputs[i], mem[i], write, tf);
                    start += size;
                }
            }
            catch (Exception e) { e.OutputError(); }
        }
        public void Update(NetworkMem[][] mem, int runsize = 16)
        {
            try
            {
                Parallel.For(0, Filters.Count, j =>
                {
                    for (int i = 0; i < Filters[j].Networks.Count(); i++)
                    {
                        mem[j][i].Update(runsize, Filters[j].GetChangeSize(), Filters[j].Networks[i]);
                    }
                });
            }
            catch (Exception e) { e.OutputError(); }
        }
    }
}