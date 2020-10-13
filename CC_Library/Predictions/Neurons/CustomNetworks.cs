using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class CustomNeuralNet
    {
        public const int DictSize = 20;
        #region Dictionary
        public static NeuralNetwork DictionaryNetwork(List<string> Words)
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(DictSize, Words.Count(), Activation.Linear));
            network.Layers.Add(new Layer(Words.Count(), network.Layers[0], Activation.SoftMax));
            network.Datatype = Datatype.Dictionary;

            double[,] Layer1Weights = new double[DictSize, Words.Count()];
            for(int i = 0; i < Words.Count(); i++)
            {
                Element e = Datatype.Dictionary.GetElement(Words[i]);
                for(int j = 0; j < e.Location.Count(); j++)
                {
                    Layer1Weights[j, i] = e.Location[j];
                }
            }

            network.Layers[0].Weights = Layer1Weights;

            return network;
        }
        public static void SaveDict(this NeuralNetwork Network, List<string>Words)
        {
            for(int i = 0; i < Words.Count(); i++)
            {
                double[] vals = new double[Network.Layers[0].Weights.GetLength(0)];
                for(int j = 0; j < Network.Layers[0].Weights.GetLength(0); j++)
                {
                    vals[j] = Network.Layers[0].Weights[j, i];
                }
                Element ele = new Element(Datatype.Dictionary, Words[i], vals);
                ele.Write();
            }
        }
        #endregion

        #region Masterformat
        public static NeuralNetwork MFNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(20, DictSize, Activation.ReLu));
            network.Layers.Add(new Layer(20, network.Layers[0], Activation.ReLu));
            network.Layers.Add(new Layer(40, network.Layers[1], Activation.SoftMax));
            network.Datatype = Datatype.Masterformat;

            return network;
        }
        #endregion

        #region OccupanctLoadFactor
        /* 1 5, 2 7, 3 11, 4 15, 5 20, 6 30, 7 35, 8 40, 9 50, 10 60, 11 100, 12 120, 13 200, 14 240, 15 300, 16 500 */
        public  static NeuralNetwork OLFNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(10, DictSize, Activation.ReLu));
            network.Layers.Add(new Layer(10, network.Layers[0], Activation.ReLu));
            network.Layers.Add(new Layer(16, network.Layers[1], Activation.SoftMax));
            network.Datatype = Datatype.OccupantLoadFactor;

            return network;
        }
        #endregion

        #region BoundaryNetworks
        public static NeuralNetwork BoundaryNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(20, 15, Activation.ReLu));
            network.Layers.Add(new Layer(20, network.Layers[0], Activation.ReLu));
            network.Layers.Add(new Layer(2, network.Layers[1], Activation.SoftMax));
            network.Datatype = Datatype.Boundary;

            return network;
        }

        public static NeuralNetwork PointerNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(20, 24, Activation.ReLu));
            network.Layers.Add(new Layer(20, network.Layers[0], Activation.ReLu));
            network.Layers.Add(new Layer(16, network.Layers[1], Activation.SoftMax));
            network.Datatype = Datatype.Pointer;
            return network;
        }

        /// <summary>
        /// Inputs for Elevation Network Are:
        /// Elevation 1 X
        /// Elevation 1 Y
        /// Elevation 1 Z (6)
        /// Elevation 2 X
        /// Elevation 2 Y
        /// Elevation 2 Z (12)
        /// Elevation 3 X
        /// Elevation 3 Y
        /// Elevation 3 Z (18)
        /// Elevation 4 X
        /// Elevation 4 Y
        /// Elevation 4 Z (24)
        /// Note: If there are less than 4 elevations, a setting of all -double.MaxValue will be used as an infill setting.
        /// Output will be
        /// Position of Elevation 1 (0 / No, 1, 2, 3, 4)
        /// Position of Elevation 2 (0 / No, 1, 2, 3, 4)
        /// Position of Elevation 3 (0 / No, 1, 2, 3, 4)
        /// Position of Elevation 4 (0 / No, 1, 2, 3, 4)
        /// The output will need to be compared with all possible sets for a given room.
        /// This means the operation may need to run 20+ times depending on the room.
        /// Elevations Markers will ALL be stored during operation, but not all will be used.
        /// The markers being used will be the ones that have the highest value for a given elevation.
        /// The output will then be centered across the selected elevations (XY center point).
        /// </summary>

        public static NeuralNetwork ElevationNetwork()
        {
            NeuralNetwork network = new NeuralNetwork();

            network.Layers.Add(new Layer(64, 200, Activation.ReLu));
            network.Layers.Add(new Layer(64, network.Layers[0], Activation.ReLu));
            network.Layers.Add(new Layer(50, network.Layers[1], Activation.Linear));
            network.Datatype = Datatype.Elevation;

            return network;
        }
        #endregion
    }
}