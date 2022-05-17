using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace CC_Library.Predictions
{
    [Serializable]
    internal class AlphaAttn
    {
        public const int size = 300;
        public const int Radius = 1;
        private const double rate = 0.1;

        public double[,] Queries { get; set; }
        public double[,] Keys { get; set; }
        public double[,] Values { get; set; }
        public AlphaAttn()
        {
            this.Queries = new double[CharSet.CharCount * (1 + (2 * Radius)), size];
            this.Queries.SetRandom();
            this.Keys = new double[CharSet.CharCount * (1 + (2 * Radius)), size];
            this.Keys.SetRandom();
            this.Values = new double[CharSet.CharCount * (1 + (2 * Radius)), size];
            this.Values.SetRandom();
        }
        public void Forward(string s, AttentionMem mem)
        {
            mem.input = s.Locate(Radius); //Size should be s.Length, CharCount * Diameter

            try { mem.Q = mem.input.Dot(Queries); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size
            try { mem.K = mem.input.Dot(Keys); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size
            try { mem.V = mem.input.Dot(Values); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size

            try {
                mem.scores = mem.Q.Dot(mem.K.Transpose());//Size should be s.Length, s.Length
                mem.scores = mem.scores.Divide(Math.Sqrt(size));
            }
            catch (Exception e) { e.OutputError(); }
            try { mem.weights = Activations.SoftMax(mem.scores); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, s.Length
            try { mem.attn = mem.weights.Dot(mem.V); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size

            try { mem.attention = mem.attn.SumRange(); } catch (Exception e) { e.OutputError(); } //Size should be size
        }
        public void Backward(AttentionMem mem, AttentionChange change, double[] dvals) //dvals Size is always size
        {
            var atndvals = dvals.Dot(mem.attn.Ones()); //returns a vector [s.Length, size]
            var Vdvals = atndvals.Transpose().Dot(mem.weights); //returns a vector [size, s.Length]
            var DV = Vdvals.Dot(mem.input).Transpose(); //size is CharCount * diameter, size

            var dweights = atndvals.Dot(mem.V.Transpose()); // Size of this is s.Length, s.Length
            dweights = Activations.InverseSoftMax(dweights, mem.weights); // Size of this is s.Length, s.Length
            var Qdvals = dweights.Dot(mem.K); //size is s.Length, size
            var Kdvals = dweights.Transpose().Dot(mem.Q); //size is s.Length, size
            var DQ = mem.input.Transpose().Dot(Qdvals); //this needs to be CharCount * diameter, size
            var DK = mem.input.Transpose().Dot(Kdvals);

            change.Q.Update(DQ, 1); 
            change.K.Update(DK, 1);
            change.V.Update(DV, 1);
        }
        public void Update(AttentionChange change, int Count)
        {
            Queries.Update(change.Q, -1 * rate * Count);
            Keys.Update(change.K, -1 * rate * Count);
            Values.Update(change.V, -1 * rate * Count);
        }
        public static AlphaAttn Load(WriteToCMDLine write)
        {
            string fn = "AlphaAttention";
            fn += ".bin";
            string Folder = "NeuralNets".GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Contains(fn)))
                {
                    var doc = Files.Where(x => x.Contains(fn)).First();
                    write("Filter read from My Docs");
                    return ReadFromBinaryFile<AlphaAttn>(doc);
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
                    return (AlphaAttn)binaryFormatter.Deserialize(stream);
                }
            }

            write("Alpha Attention Not Found. New Filter Created");
            return new AlphaAttn();
        }

        public void Save(string Folder)
        {
            string FileName = Folder + "\\AlphaAttention.bin";
            WriteToBinaryFile(FileName, this, true);
        }
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Create(filePath))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        private static AlphaAttn ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (AlphaAttn)binaryFormatter.Deserialize(stream);
            }
        }
    }
    internal class AttentionChange
    {
        public double[,] Q;
        public double[,] K;
        public double[,] V;

        public AttentionChange()
        {
            this.Q = new double[CharSet.CharCount * (1 + (2 * AlphaAttn.Radius)), AlphaAttn.size];
            this.K = new double[CharSet.CharCount * (1 + (2 * AlphaAttn.Radius)), AlphaAttn.size];
            this.V = new double[CharSet.CharCount * (1 + (2 * AlphaAttn.Radius)), AlphaAttn.size];
        }
    }
    internal class AttentionMem
    {
        public double[,] input;

        public double[,] Q;
        public double[,] K;
        public double[,] V;

        public double[,] scores;
        public double[,] weights;
        public double[,] attn;

        public double[] attention;
    }
}
