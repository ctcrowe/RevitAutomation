using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;

namespace CC_Library
{
    public delegate void WriteOutput(string s);
    public class GenPredictions
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Input = directory + "\\CC_MasterformatPredictor.xml";
        private static string Output = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static string Run(string s, WriteOutput wo)
        {
            List<string> x = s.SplitTitle();
            List<PredictionElement> pes = new List<PredictionElement>();
            var assembly = typeof(GenPredictions).GetTypeInfo().Assembly;
            string name = assembly.GetManifestResourceNames().Where(z => z.EndsWith("MasterformatPredictor.xml")).First();
            string outfile = Output + "//" + name.Split('.')[name.Split('.').Count() - 2] + ".xml";
            if (File.Exists(outfile))
                File.Delete(outfile);
            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                var xdoc = new XmlDocument();
                xdoc.Load(stream);
                XDocument doc = xdoc.ToXDocument();
                foreach (string y in x)
                {
                    if (doc.Root.Elements().Any(z => z.Attribute("WORD").Value == y))
                    {
                        pes.Add(new PredictionElement(doc.Root.Elements().Where(z => z.Attribute("WORD").Value == y).First()));
                    }
                }
            }
            string prediction = pes.Predict();
            wo(prediction);
            return prediction;
        }
    }
}
