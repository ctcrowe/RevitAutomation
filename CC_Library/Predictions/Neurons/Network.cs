using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    internal class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }
        public Datatype Datatype { get; set; }

        public NeuralNetwork()
        {
            this.Layers = new List<Layer>();
        }
        public static NeuralNetwork NewNeuralNet(Datatype datatype)
        {
            switch(datatype)
            {
                default:
                case Datatype.Masterformat:
                    return CustomNeuralNet.MFNetwork();
                case Datatype.Boundary:
                    return CustomNeuralNet.BoundaryNetwork();
                case Datatype.Elevation:
                    return CustomNeuralNet.ElevationNetwork();
                case Datatype.Pointer:
                    return CustomNeuralNet.PointerNetwork();
            }
        }

        public NeuralNetwork(XDocument doc, Datatype datatype)
        {
            this.Datatype = datatype;
            this.Layers = new List<Layer>();
            for(int i = 0; i < doc.Root.Elements("Layer").Count(); i++)
            {
                Layers.Add(new Layer(doc.Root.Elements("Layer").Where(x => int.Parse(x.Attribute("Number").Value) == i).First()));
            }
        }

        public double[,] LastResult(int Outputs)
        {
            double[,] Result = new double[Outputs, Layers.Last().Weights.GetLength(0)];
            for(int i = 0; i < Result.GetLength(0); i++)
            {
                for(int j = 0; j < Result.GetLength(1); j++)
                {
                    Result[i, j] = 1;
                }
            }
            return Result;
        }

        public static void Save(NeuralNetwork network, List<string> Words)
        {
            string Folder = "NeuralNets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            string FileName = "NeuralNet_" + network.Datatype.ToString() + ".xml";
            XDocument doc = new XDocument(new XElement(network.Datatype.ToString()))
            { Declaration = new XDeclaration("1.0", "utf-8", "yes") };

            for(int i = 0; i < network.Layers.Count(); i++)
            {
                doc.Root.Add(network.Layers[i].WriteXml());
            }

            doc.Save(Folder + "\\" + FileName);
        }
    }
    internal static class LoadNeuralNetwork
    {
        public static NeuralNetwork LoadNetwork(this Datatype datatype, List<string>Words, WriteToCMDLine write)
        {
            if(datatype == Datatype.Dictionary)
            {
                return CustomNeuralNet.DictionaryNetwork(Words);
            }
            string Folder = "NeuralNets".GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Split('\\').Last() == "NeuralNet_" + datatype.ToString() + ".xml"))
                {
                    var xdoc = Files.Where(x => x.Split('\\').Last() == "NeuralNet_" + datatype.ToString() + ".xml").First();
                    write("Loaded from MyDocs");
                    return new NeuralNetwork(XDocument.Load(xdoc), datatype);
                }
            }
            var assembly = typeof(ReadWriteXML).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains("NeuralNet_" + datatype.ToString() + ".xml")))
            {
                string name = assembly.GetManifestResourceNames().Where(x => x.Contains("NeuralNet_" + datatype.ToString() + ".xml")).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    write("Loaded From Assembly");
                    return new NeuralNetwork(XDocument.Load(stream), datatype);
                }
            }
            write("New Neural Net");
            return NeuralNetwork.NewNeuralNet(datatype);
        }
    }
}
