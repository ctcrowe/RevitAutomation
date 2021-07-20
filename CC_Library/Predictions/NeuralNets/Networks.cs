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
                case Datatype.Alpha:
                    return AlphaNetwork();
                case Datatype.AlphaContext:
                    return AlphaContextNetwork();
                case Datatype.Dictionary:
                    return DictNetwork();
                case Datatype.ObjectStyle:
                    return ObjectStyleNetwork();
                case Datatype.OccupantLoadFactor:
                    return LoadFactorNetwork();
            }
        }
        private static NeuralNetwork AlphaNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.Alpha);
            network.Layers.Add(new Layer(Alpha.DictSize, 3 * Alpha.CharCount(), Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.Linear));
            return network;
        }
        private static NeuralNetwork AlphaContextNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.AlphaContext);
            network.Layers.Add(new Layer(1, Alpha.CharCount() * Alpha.SearchSize, Activation.Linear));
            return network;
        }
        private static NeuralNetwork DictNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.Dictionary);

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.ReLu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Enum.GetNames(typeof(Dict)).GetLength(0), network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork MFNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.Masterformat);

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(40, network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork ObjectStyleNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.ObjectStyle);

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize + 18, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Enum.GetNames(typeof(ObjectCategory)).Count(), network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork LoadFactorNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.OccupantLoadFactor);

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize + 1, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Enum.GetNames(typeof(ObjectCategory)).Count(), network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
    }
}
