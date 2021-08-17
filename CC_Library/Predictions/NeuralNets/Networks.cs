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
                case Datatype.AlphaContextPrimary:
                    return AlphaContextNetwork(0);
                case Datatype.AlphaContextSecondary:
                    return AlphaContextNetwork(1);
                case Datatype.AlphaContextTertiary:
                    return AlphaContextNetwork(2);
                case Datatype.ObjectStyle:
                    return ObjectStyleNetwork();
                case Datatype.OccupantLoadFactor:
                    return LoadFactorNetwork();
                case Datatype.LineWeight:
                    return LineWeightNetwork();
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
        private static NeuralNetwork AlphaContextNetwork(int i = 0)
        {
            NeuralNetwork network;
            switch(i)
            {
                default:
                case 0: network = new NeuralNetwork(Datatype.AlphaContextPrimary); break;
                case 1: network = new NeuralNetwork(Datatype.AlphaContextSecondary); break;
                case 2: network = new NeuralNetwork(Datatype.AlphaContextTertiary); break;
            }
            network.Layers.Add(new Layer(1, Alpha.CharCount() * Alpha.SearchSize, Activation.Linear));
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

            network.Layers.Add(new Layer(Alpha.DictSize, (2 * Alpha.DictSize) + 18, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Enum.GetNames(typeof(ObjectCategory)).Count(), network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork LineWeightNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.ObjectStyle);

            network.Layers.Add(new Layer(Alpha.DictSize, 2 * Alpha.DictSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(16, network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork LoadFactorNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.OccupantLoadFactor);

            network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize + 1, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Enum.GetNames(typeof(OccLoadFactor)).Count(), network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
    }
}
