using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;
/*
namespace CC_Library.Predictions
{
    internal class GlobalContext
    {
        private Datatype datatype { get; }
        public NeuralNetwork Network { get; }

        internal GlobalContext(Datatype dt, WriteToCMDLine write)
        {
            datatype = dt;
            Network = Datatype.GlobalContext.LoadSpecialNetwork(dt, write);
        }
        public void Save()
        {
            Network.Save(datatype);
        }

        public double[] Contextualize(double[] inputs, AlphaMem am, WriteToCMDLine write)
        {
            am.GlobalContextOutputs.Add(inputs);
            for(int i = 0; i < Network.Layers.Count(); i++)
            {
                am.GlobalContextOutputs.Add(Network.Layers[i].Output(am.GlobalContextOutputs.Last()));
            }
            return am.GlobalContextOutputs.Last();
        }
        public double[] Backward(double[] DValues, AlphaMem am, WriteToCMDLine write)
        {
            for(int i = Network.Layers.Count() - 1; i >= 0; i--)
            {
                DValues = Network.Layers[i].DActivation(DValues, am.GlobalContextOutputs[i + 1]);
                Network.Layers[i].DBiases(DValues);
                Network.Layers[i].DWeights(DValues, am.GlobalContextOutputs[i]);
                DValues = Network.Layers[i].DInputs(DValues);
            }
            return DValues;
        }
    }
}*/