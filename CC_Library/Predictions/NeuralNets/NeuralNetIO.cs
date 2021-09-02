using System.Linq;
using CC_Library.Datatypes;
using System.IO;
using System.Reflection;

namespace CC_Library.Predictions
{
    internal static class ReadWriteNeuralNetwork
    {
        public static void Save(this NeuralNetwork network, Datatype reference = Datatype.None)
        {
            string Folder = "NeuralNets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            string FileName = Folder + "\\NeuralNet_" + network.Datatype;
            if(reference != Datatype.None)
                FileName += "_" + reference.ToString();
            FileName += ".bin";
            WriteToBinaryFile(FileName, network, true);
        }
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Create(filePath))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        public static T ReadFromBinaryFile<T>(this string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
        public static NeuralNetwork LoadNetwork(this Datatype datatype, Datatype reference = Datatype.None)
        {
            string fn = "NeuralNet_";
            if(reference != Datatype.None)
                fn += reference.ToString() + "_";
            fn += datatype.ToString() + ".bin";
            string Folder = "NeuralNets".GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Contains(fn)))
                {
                    var doc = Files.Where(x => x.Contains(fn)).First();
                    return ReadFromBinaryFile<NeuralNetwork>(doc);
                }
            }
            var assembly = typeof(ReadWriteNeuralNetwork).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(fn)))
            {
                string name = assembly.GetManifestResourceNames().Where(x => x.Contains(fn)).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (NeuralNetwork)binaryFormatter.Deserialize(stream);
                }
            }
            /*
            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(y => !y.IsInterface).Where(x => type.IsAssignableFrom(x)).ToList();
            for (int i = 0; i < NetTypes.Count(); i++)
            {
                var Network = (INetworkPredUpdater)Activator.CreateInstance(NetTypes[i]);
                if (Network.datatype == dt)
                {
                    entry.DesiredOutput = new double[Network.Network.Layers.Last().Biases.Count()];
                    entry.DesiredOutput[correct] = 1;
                    Network.Propogate(entry);
                    break;
                }
            }*/
            return NeuralNets.NewNeuralNet(datatype);
        }
    }
}
