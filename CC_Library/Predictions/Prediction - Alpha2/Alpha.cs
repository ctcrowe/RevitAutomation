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
        private List<IAlphaTransformer> Transformers { get; }
        internal Alpha2(WriteToCMDLine write)
        {
            this.Transformers = new List<IAlphaTransformer>();
            Transformers.Add(new Transformer1());
            //Filters.Add(new AlphaFilter3(write));
            //Filters.Add(new WordFilter(write));
            //Filters.Add(new WordFilter2(write));
        }
        public int GetSize()
        {
            int size = 0;
            for (int i = 0; i < Transformers.Count(); i++)
            {
                size += Transformers[i].Size;
            }
            return size;
        }
        public void Save()
        {
            string Folder = "NeuralNets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            Parallel.For(0, this.Transformers.Count(), i => this.Transformers[i].Save(Folder));
        }
        public void Load(WriteToCMDLine write)
        {
            Parallel.For(0, this.Transformers.Count(), i => this.Transformers[i] = this.Transformers[i].LoadAlpha(write));
        }
        public AttentionMem[] Forward(string s, WriteToCMDLine write)
        {
            AttentionMem[] result = new AttentionMem[Transformers.Count()];
            try
            {
                for (int i = 0; i < Transformers.Count(); i++)
                {
                    result[i] = new AttentionMem();
                    Transformers[i].Forward(s, result[i]);
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
                for (int i = 0; i < Transformers.Count(); i++)
                {
                    var dvals = DValues.ToList().GetRange(start, Transformers[i].Size).ToArray();
                    Transformers[i].Backward(outputs[i], change[i], dvals);
                    start += Transformers[i].Size;
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
