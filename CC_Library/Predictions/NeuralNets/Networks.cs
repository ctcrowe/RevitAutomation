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
                case Datatype.ObjectStyle:
                    return ObjectStyleNetwork();
                case Datatype.CutLineWeight:
                    return CutLineWeightNetwork();
                case Datatype.ProjectedLineWeight:
                    return ProjectedLineWeightNetwork();
                case Datatype.CategoryVisibility:
                    return CategoryVisibilityNetwork();
            }
        }
        private static NeuralNetwork ObjectStyleNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.ObjectStyle);

            network.Layers.Add(new Layer(Alpha.DictSize, (2 * Alpha.DictSize) + 18, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(Enum.GetNames(typeof(ObjectCategory)).Count(), network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork CutLineWeightNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.CutLineWeight);

            network.Layers.Add(new Layer(Alpha.DictSize, 2 * Alpha.DictSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(17, network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork ProjectedLineWeightNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.ProjectedLineWeight);

            network.Layers.Add(new Layer(Alpha.DictSize, 2 * Alpha.DictSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(17, network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
        private static NeuralNetwork CategoryVisibilityNetwork()
        {
            NeuralNetwork network = new NeuralNetwork(Datatype.CategoryVisibility);

            network.Layers.Add(new Layer(Alpha.DictSize, 2 * Alpha.DictSize, Activation.LRelu));
            network.Layers.Add(new Layer(Alpha.DictSize, network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
            network.Layers.Add(new Layer(2, network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));

            return network;
        }
    }
}