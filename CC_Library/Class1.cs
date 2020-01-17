using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library
{
    public class WordSections
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string xfile = directory + "\\CC_XMLDictionary.xml";
        private const readonly double Distance = 75;

        public static List<string> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<string>();

            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        List<string> title = TitleAnalysis.SplitTitle(ele);
                        foreach (string s in title)
                            if (!data.Contains(s))
                                data.Add(s);
                    }
                }
            }
            if (File.Exists(xfile))
            {
                XDocument doc = XDocument.Load(xfile);
                foreach (XElement ele in doc.Root.Elements())
                {
                    data.Add(ele.Attribute("Value").Value);
                }
            }
            return data;
        }
        public static void CopyToXml(string file, string n)
        {
            string[] lines = File.ReadAllLines(file);
            XDocument doc = new XDocument(new XElement("DICTIONARY")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            foreach (string s in lines)
            {
                XElement e = new XElement("string");
                e.Add(new XAttribute("Value", s));
            }
            doc.Save(xfile);
        }
        public static void GeneratePrediction(string f1)
        {
            var Input = new List<TitleAnalysis>();
            var Output = new List<Prediction>();

            string[] lines = File.ReadAllLines(f1);
            foreach (string l in lines)
                Input.Add(new TitleAnalysis(l.Split('\t').First(), int.Parse(l.Split('\t')[1])));
            XDocument doc = XDocument.Load(xfile);
            foreach (XElement ele in doc.Root.Elements())
                Output.Add(new Prediction(ele.Attribute("Value").Value));
            while (true)
            {
                foreach (var o in Output)
                {
                    var connections = Input.Where(x => x.Title.Contains(o.Word)).ToList();
                    foreach (var c in connections)
                    {
                        double[] Prediction = new double[26];
                        var textset = Output.Where(x => c.Title.Contains(x.Word) && x.Word != o.Word).ToList();
                        for(int z = 0; z < 27, z++)
                        {
                            double a = 0;
                            foreach(var x in textset)
                            {
                                double v = textset.Value[z] * textset.Value[z];
                                a += v;
                            }
                            a /= textset.Count();
                            Prediction[z] = Math.Sqrt(a);
                        }
                        double m = Prediction.Max();
                        int p = Array.IndexOf(Prediction, m);
                        if(p == c.Section)
                        {
                            if(m < Distance)
                            {
                            }
                        }
                        else
                        {
                            if(Prediction.Any(x => x > Distance))
                            {
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
        }
        public static void run()
        {
            string filedir = string.Empty;
            string filename = string.Empty;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filedir = ofd.FileName.TrimEnd(ofd.FileName.Split('\\').Last().ToCharArray());
                }
                Console.WriteLine(filedir);
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = "c:\\";
                sfd.Filter = "All files (*.*)|*.*";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (sfd.FileName.EndsWith(".txt"))
                        filename = sfd.FileName;
                    else
                        filename = sfd.FileName.Split('.').FirstOrDefault() + ".txt";
                }
                Console.WriteLine(filename);
            }
            var data = GetData(filedir);
            File.WriteAllLines(filename, data);
        }
    }
}
