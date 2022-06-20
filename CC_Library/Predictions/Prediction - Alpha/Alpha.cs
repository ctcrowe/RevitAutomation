using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Collections.Generic;
using System.IO;

namespace CC_Library.Predictions
{
    public class Alpha
    {
        public const int _Outputs = 500;
        internal Transformer Xfmr { get; }
        public Alpha(string type, WriteToCMDLine write)
        {
            this.Xfmr = (type + "Alpha").LoadXfmr(CharSet.CharCount * 3, _Outputs, 200, write);
        }
        public void Save()
        {
            string Folder = "NeuralNets".GetMyDocs();
            Xfmr.Save(Folder);
        }
        internal AttentionMem GetMem() { return new AttentionMem(); }
        internal AttentionChange GetChange() { return new AttentionChange(Xfmr); }
        internal double[,] Forward(string s, AttentionMem mem, WriteToCMDLine write)
        {
            var _input = s.Locate(1);
            Xfmr.Forward(_input, mem);
            return mem.attn;
        }
        public double[,] Forward(string s) { return Xfmr.Forward(s.Locate(1)); }
        internal void Backward(double[,] DValues, AttentionMem outputs, AttentionChange change, WriteToCMDLine write) { Xfmr.Backward(outputs, change, DValues); }
        internal void Update(AttentionChange change, WriteToCMDLine write)
        {
            Xfmr.Update(change, write);
            Xfmr.Save("NeuralNets".GetMyDocs());
        }
    }
}
