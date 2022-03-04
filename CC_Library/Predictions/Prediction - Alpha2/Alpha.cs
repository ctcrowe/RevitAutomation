﻿using System;
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
            Filters.Add(new AlphaFilter3(write));
            Filters.Add(new LongTermWordFilter(write));
            Filters.Add(new WordFilter(write));
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
        public AlphaMem[] CreateAlphaMemory(string s, NeuralNetwork net = null)
        {
            net = net == null ? Predictionary.GetNetwork(CMDLibrary.WriteNull) : net;
            AlphaMem[] mem = new AlphaMem[Filters.Count];
            Parallel.For(0, Filters.Count, j => mem[j] = new AlphaMem(Filters[j].GetLength(s, net)));
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
        public double[] Forward(string s, AlphaMem[] am, WriteToCMDLine write, NeuralNetwork net = null)
        {
            try
            {
                net = net == null ? Predictionary.GetNetwork(CMDLibrary.WriteNull) : net;
                if (am.Length == Filters.Count())
                {
                    List<double> output = new List<double>();
                    for (int i = 0; i < Filters.Count(); i++)
                    {
                        output.AddRange(Filters[i].Forward(s, am[i], net));
                    }
                    return output.ToArray();
                }
                else
                    return null;
            }
            catch (Exception e) { e.OutputError(); }
            return null;
        }
        public void Backward(string s, double[] DValues, AlphaMem[] am, NetworkMem[,] mem, WriteToCMDLine write, NeuralNetwork net = null)
        {
            try
            {
                net = net == null ? Predictionary.GetNetwork(CMDLibrary.WriteNull) : net;
                int start = 0;
                for(int i = 0; i < Filters.Count(); i++)
                {
                    var size = Filters[i].GetSize();
                    var dvals = DValues.ToList().GetRange(start, size).ToArray();
                    Filters[i].Backward(s, dvals, am[i], mem[i, 0], mem[i, 1], net);
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
