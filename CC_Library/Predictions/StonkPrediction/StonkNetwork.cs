﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public class Stonk
    {
        /// <summary>
        /// index of stonk in enum [enum size]
        /// % change of stonk (price at close - price at open) / price at close
        /// variability ratio (high - low) / price at close
        /// secondary ratio (price at close - vwap / price at close
        /// hours since change
        /// Time of Day = double.Parse(TimeUTC Hours) / 24
        /// Volume
        /// </summary>
        public Stonk()
        {
            Network = Datatype.Stonk.LoadNetwork(new WriteToCMDLine(CMDLibrary.WriteNull));
            if (Network.Datatype == Datatype.None)
            {
                Network = new NeuralNetwork(Datatype.Stonk);
                Network.Layers.Add(new Layer(MktSize, 8, Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.Linear));
            }
        }

        public const int MktSize = 30;
        public NeuralNetwork Network { get; }

        public double[] Forward(List<Comparison> vals, StonkContext context)
        {
            double[] ctxt = new double[vals.Count()];
            double[,] loc = new double[vals.Count(), MktSize];

            Parallel.For(0, vals.Count(), j =>
            {
                double[] a = vals[j].Values.Duplicate();
                for (int i = 0; i < Network.Layers.Count(); i++) { a = Network.Layers[i].Output(a); }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(vals[j]);
            });

            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public double[] Forward(List<Comparison> vals, StonkContext ctxt, StonkMem sm)
        {
            Parallel.For(0, vals.Count(), j =>
            {
                sm.LocationOutputs[j].Add(vals[j].Values.Duplicate());
                for (int i = 0; i < Network.Layers.Count(); i++)
                {
                    sm.LocationOutputs[j].Add(Network.Layers[i].Output(sm.LocationOutputs[j].Last()));
                }
                ctxt.Contextualize(vals[j], j, sm);
            });
            return sm.Multiply();
        }
        public void Backward(double[] DValues, StonkContext context, StonkMem sm, NetworkMem mem, NetworkMem CtxtMem)
        {
            var LocDValues = sm.DLocation(DValues);
            DValues = sm.DGlobalContext(DValues);
            DValues = Activations.InverseSoftMax(DValues, sm.GlobalOutputs.ToArray());
            context.Backward(DValues, sm.LocationOutputs.Count(), sm, CtxtMem);
            Parallel.For(0, sm.GlobalOutputs.Count(), j =>
            {
                var ldv = LocDValues[j];
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    ldv = mem.Layers[i].DActivation(ldv, sm.LocationOutputs[j][i + 1]);
                    mem.Layers[i].DBiases(ldv, Network.Layers[i], sm.GlobalOutputs.Count());
                    mem.Layers[i].DWeights(ldv, sm.LocationOutputs[j][i], Network.Layers[i], sm.GlobalOutputs.Count());
                    ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}
