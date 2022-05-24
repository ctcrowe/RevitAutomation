using System.Linq;
using CC_Library.Datatypes;
using System.IO;
using System.Reflection;

namespace CC_Library.Predictions
{
    internal static class ReadWriteAlphaXfmr
    {
        public static void Save(this Transformer filter, string Folder)
        {
            string FileName = Folder + "\\" + filter.Name + ".bin";
            WriteToBinaryFile(FileName, filter, true);
        }
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Create(filePath))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        private static T ReadFromBinaryFile<T>(this string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
        public static Transformer LoadXfmr(this string _name, int _InputSize, int _ValueSize, int _QuerySize, WriteToCMDLine write)
        {
            string fn = _name;
            fn += ".bin";
            string Folder = "NeuralNets".GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Contains(fn)))
                {
                    var doc = Files.Where(x => x.Contains(fn)).First();
                    write("Filter read from My Docs");
                    var XFMR = ReadFromBinaryFile<Transformer>(doc);
                    XFMR.Name = _name;
                    return XFMR;
                }
            }
            var assembly = typeof(ReadWriteNeuralNetwork).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(fn)))
            {
                string name = assembly.GetManifestResourceNames().Where(x => x.Contains(fn)).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    write("Filter Read from Assembly");
                    var XFMR = (Transformer)binaryFormatter.Deserialize(stream);
                    XFMR.Name = _name;
                    return XFMR;
                }
            }
                          
            write("Filter " + fn + " Not Found. New Filter Created");
            return new Transformer(_name, _InputSize, _ValueSize, _QuerySize);
        }
    }
}
