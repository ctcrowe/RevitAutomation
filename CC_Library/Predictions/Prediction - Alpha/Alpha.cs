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
        private List<Transformer> Xfmrs { get; }
        internal Alpha2(WriteToCMDLine write)
        {
            this.Xfmrs = new List<Transformer>();
            Xfmrs.Add("XfmrAlpha1".LoadAlpha(400, write));
            //Filters.Add(new AlphaFilter3(write));
            //Filters.Add(new WordFilter(write));
            //Filters.Add(new WordFilter2(write));
        }
        public int GetSize()
        {
            int size = 0;
            for (int i = 0; i < Xfmrs.Count(); i++)
            {
                size += Xfmrs[i].Size;
            }
            return size;
        }
        public void Save()
        {
            string Folder = "NeuralNets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            Parallel.For(0, this.Xfmrs.Count(), i => this.Xfmrs[i].Save(Folder));
        }
        public AttentionMem[] Forward(string s, WriteToCMDLine write)
        {
            AttentionMem[] result = new AttentionMem[Xfmrs.Count()];
            try
            {
                for (int i = 0; i < Xfmrs.Count(); i++)
                {
                    result[i] = new AttentionMem();
                    Xfmrs[i].Forward(s, result[i]);
                }
            }
            catch (Exception e) { e.OutputError(); }
            return result;
        }
        public void Backward(double[] DValues, AttentionMem[] outputs, AttentionChange[] change, WriteToCMDLine write, bool tf = false)
        {
            var start = 0;
            try
            {
                for (int i = 0; i < Xfmrs.Count(); i++)
                {
                    var dvals = DValues.ToList().GetRange(start, Xfmrs[i].Size).ToArray();
                    Xfmrs[i].Backward(outputs[i], change[i], dvals);
                    start += Xfmrs[i].Size;
                }
            }
            catch (Exception e) { e.OutputError(); }
        }
        public void Update(NetworkMem[][] mem, int runsize = 16)
        {
            /*
            try
            {
                Parallel.For(0, Transformers.Count, j =>
                {
                    for (int i = 0; i < Transformers[j].Networks.Count(); i++)
                    {
                        mem[j][i].Update(runsize, 0.1, Transformers[j].Networks[i]);
                    }
                });
            }
            catch (Exception e) { e.OutputError(); }*/
        }
    }
}
