using System;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class NeuralNets
    {
        public static NeuralNetwork NewNeuralNet(this Datatype datatype)
        {
            switch(datatype)
            {
                default:
                case Datatype.Masterformat:
                    return MFNetwork();
                case Datatype.Div8Type:
                    return Div8TypeNetwork();
                case Datatype.Alpha:
                    return AlphaNetwork();
                case Datatype.AlphaContext:
                    return AlphaContextNetwork();
                case Datatype.Dictionary:
                    return DictNetwork();
            }
        }


        private static NeuralNetwork AlphaNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();
            network.Layers.Add(new Layer(Alpha.DictSize, 3 * Alpha.CharCount(), Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last(), Activation.Linear));
            network.Datatype = Datatype.Alpha;
            return network;
        }
        /*
        private static NeuralNetwork LocalContextNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();
            network.Layers.Add(new Layer(Alpha.CharCount(), Alpha.CharCount() * Alpha.SearchSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.CharCount(), network.Layers.Last(), Activation.LRelu));
            network.Layers.Add(new Layer(1, network.Layers.Last(), Activation.LRelu));
            network.Datatype = Datatype.LocalContext;
            return network;
        }*/
        private static NeuralNetwork AlphaContextNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();
            network.Layers.Add(new Layer(1, Alpha.CharCount() * Alpha.SearchSize, Activation.Linear));
            network.Datatype = Datatype.AlphaContext;
            return network;
        }
        private static NeuralNetwork DictNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.ReLu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last(), Activation.LRelu));
            network.Layers.Add(new Layer(Enum.GetNames(typeof(Dict)).GetLength(0), network.Layers.Last(), Activation.CombinedCrossEntropySoftmax));
            network.Datatype = Datatype.Dictionary;

            return network;
        }
        #region Div8
        private static NeuralNetwork MFNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last(), Activation.LRelu));
            network.Layers.Add(new Layer(40, network.Layers.Last(), Activation.CombinedCrossEntropySoftmax));
            network.Datatype = Datatype.Masterformat;

            return network;
        }
        /// <summary>
        /// Types Are
        /// 0 - Door
        /// 1 - Window
        /// 2 - Other
        /// 3 - Subtype
        /// </summary>
        /// <returns></returns>
        private static NeuralNetwork Div8TypeNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last(), Activation.LRelu));
            network.Layers.Add(new Layer(4, network.Layers.Last(), Activation.CombinedCrossEntropySoftmax));
            network.Datatype = Datatype.Div8Type;

            return network;
        }
        #endregion
    }
}