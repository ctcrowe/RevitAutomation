using System.Linq;
using CC_Library.Datatypes;
using System.IO;
using System.Reflection;

namespace CC_Library.Predictions
{
    internal static class ReadWriteNeuralNetwork
    {
        public static void Save(this NeuralNetwork network, Datatype reference = Datatypes.None)
        {
            string Folder = "NeuralNets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            string FileName = Folder + "\\NeuralNet_" + network.Datatype.ToString();
            if(reference != Datatype.None)
                FileName += "_" + reference.Datatype.ToString();
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
        public static NeuralNetwork LoadNetwork(this Datatype datatype, WriteToCMDLine write)
        {
            string fn = "NeuralNet_" + datatype.ToString() + ".bin";
            string Folder = "NeuralNets".GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Contains(fn)))
                {
                    var doc = Files.Where(x => x.Contains(fn)).First();
                    write("Loaded from MyDocs");
                    return ReadFromBinaryFile<NeuralNetwork>(doc);
                }
            }
            var assembly = typeof(ReadWriteXML).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(fn)))
            {
                string name = assembly.GetManifestResourceNames().Where(x => x.Contains(fn)).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    write("Loaded From Assembly");
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (NeuralNetwork)binaryFormatter.Deserialize(stream);
                }
            }
            write("New Neural Net");
            return NeuralNets.NewNeuralNet(datatype);
        }
        public static NeuralNetwork LoadSpecialNetwork(this Datatype datatype, Datatype reference, WriteToCMDLine write)
        {
            string fn = "NeuralNet_" + reference.ToString() + "_" + datatype.ToString() + ".bin";
            string Folder = "NeuralNets".GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Contains(fn)))
                {
                    var doc = Files.Where(x => x.Contains(fn)).First();
                    write("Loaded from MyDocs");
                    return ReadFromBinaryFile<NeuralNetwork>(doc);
                }
            }
            var assembly = typeof(ReadWriteXML).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(fn)))
            {
                string name = assembly.GetManifestResourceNames().Where(x => x.Contains(fn)).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    write("Loaded From Assembly");
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (NeuralNetwork)binaryFormatter.Deserialize(stream);
                }
            }
            write("New Neural Net");
            return NeuralNets.NewNeuralNet(datatype);
        }
    }
}
