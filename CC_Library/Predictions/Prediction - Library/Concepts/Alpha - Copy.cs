﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Collections.Generic;
using System.IO;

namespace CC_Library.Predictions
{
    internal class Alpha2
    {
        public const int _Outputs = 500;
        private List<Transformer> Xfmrs { get; }
        internal Alpha2(WriteToCMDLine write)
        {
            this.Xfmrs = new List<Transformer>();
            Xfmrs.Add("XfmrAlpha1".LoadXfmr(CharSet.CharCount * 3, _Outputs, 200, write, 0, 200));
            Xfmrs.Add("XfmrAlpha2".LoadXfmr(CharSet.CharCount * 3, _Outputs, 200, write, 100, 200));
            Xfmrs.Add("XfmrAlpha3".LoadXfmr(CharSet.CharCount * 3, _Outputs, 200, write, 0, 100));
            Xfmrs.Add("XfmrAlpha4".LoadXfmr(CharSet.CharCount * 3, _Outputs, 200, write, 300, 0));
            Xfmrs.Add("XfmrAlpha5".LoadXfmr(CharSet.CharCount * 3, _Outputs, 200, write, 200, 200));
        }
        public void Save()
        {
            string Folder = "NeuralNets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            Parallel.For(0, this.Xfmrs.Count(), i => this.Xfmrs[i].Save(Folder));
        }
        public AttentionMem[] GetMem()
        {
            AttentionMem[] mem = new AttentionMem[this.Xfmrs.Count()];
            Parallel.For(0, mem.Count(), j => mem[j] = new AttentionMem());
            return mem;
        }
        public AttentionChange[] GetChange()
        {
            AttentionChange[] change = new AttentionChange[this.Xfmrs.Count()];
            Parallel.For(0, change.Count(), j => change[j] = new AttentionChange(Xfmrs[j]));
            return change;
        }
        public double[,] Forward(string s, AttentionMem[] mem, WriteToCMDLine write)
        {
            var _input = s.Locate(1);
            double[,] result = new double[Xfmrs.Count() * s.Length, _Outputs];
            try
            {
                Parallel.For(0, Xfmrs.Count(), j =>
                {
                    Xfmrs[j].Forward(_input, mem[j]);
                    Parallel.For(0, s.Length, i => result.SetRank(mem[j].attn.GetRank(i), i + (j * s.Length)));
                });
            }
            catch (Exception e) { e.OutputError(); }
            return result;
        }
        public double[,] Forward(string s, WriteToCMDLine write)
        {
            var _input = s.Locate(1);
            double[,] result = new double[Xfmrs.Count() * s.Length, _Outputs];
            try
            {
                Parallel.For(0, Xfmrs.Count(), j =>
                {
                    var f = Xfmrs[j].Forward(_input);
                    Parallel.For(0, s.Length, i => result.SetRank(f.GetRank(i), i + (j * s.Length)));
                });
            }
            catch (Exception e) { e.OutputError(); }
            return result;
        }
        public void Backward(double[,] DValues, AttentionMem[] outputs, AttentionChange[] change, WriteToCMDLine write)
        {
            int length = DValues.GetLength(0) / Xfmrs.Count();
            try
            {
                Parallel.For(0, Xfmrs.Count(), j =>
                {
                    var dvals = new double[length, _Outputs];
                    Parallel.For(0, length, i =>
                    {
                        var inputs = DValues.GetRank(i + (j * length));
                        inputs.Divide(length);
                        dvals.SetRank(inputs, i);
                    });
                    Xfmrs[j].Backward(outputs[j], change[j], dvals);
                });
            }
            catch (Exception e) { e.OutputError(); }
        }
        public void Update(AttentionChange[] change, int[] numbs, WriteToCMDLine write)
        {
            try
            {
                Parallel.For(0, Xfmrs.Count, j =>
                {
                    if (numbs.Contains(j))
                    {
                        Xfmrs[j].Update(change[j], write);
                        Xfmrs[j].Save("NeuralNets".GetMyDocs());
                    }
                });
            }
            catch (Exception e) { e.OutputError(); }
        }
    }
}
