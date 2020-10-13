using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal class Element
    {
        public string Label;
        public double[] Location;
        public Datatype datatype;

        public Element(Datatype dt, string Name)
        {
            this.Label = Name;
            this.datatype = dt;
            Random r = new Random();

            double[] Loc = new double[CustomNeuralNet.DictSize];
            for (int i = 0; i < CustomNeuralNet.DictSize; i++)
            {
                double swap = r.NextDouble();
                double v = r.NextDouble();
                if (swap > 0.50)
                    Loc[i] = v;
                else
                    Loc[i] = -v;
            }

            this.Location = Loc;
        }
        public Element(Datatype dt, string Name, double[] values)
        {
            this.Label = Name;
            this.datatype = dt;
            this.Location = values;
        }
        public Element(XDocument doc)
        {
            this.Label = doc.Root.Attribute("Label").Value;
            this.datatype = (Datatype)Enum.Parse(typeof(Datatype), doc.Root.Attribute("Datatype").Value);
            double[] Loc = new double[CustomNeuralNet.DictSize];
            for (int i = 0; i < CustomNeuralNet.DictSize; i++)
            {
                if (doc.Root.Elements("Data").Any(x => x.Attribute("Number").Value == i.ToString()))
                {
                    XElement ele = doc.Root.Elements("Data").Where(x => int.Parse(x.Attribute("Number").Value) == i).First();
                    Loc[i] = double.Parse(ele.Attribute("Location").Value);
                }
                else
                {
                    Random r = new Random();
                    Loc[i] = r.NextDouble();
                }
            }
            this.Location = Loc;
        }
        public void AdjustElement(double[] change)
        {
            double rate = 1e-5;
            for(int i = 0; i < this.Location.Count(); i++)
            {

                if (change[i] > 1 || change[i] < -1)
                    if (change[i] > 1)
                        this.Location[i] -= rate;
                    else
                        this.Location[i] += rate;
                else
                    this.Location[i] -= (rate * change[i]);
            }
            Write();
        }
        public void Write()
        {
            string Dir = datatype.ToString().GetMyDocs();
            string FN = this.datatype.ToString() + "_" + Label + ".xml";
            string Filename = Dir + "\\" + FN;
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            XDocument doc = new XDocument(new XElement(datatype.ToString()))
                { Declaration = new XDeclaration("1.0", "utf-8", "yes") };

            doc.Root.Add(new XAttribute("Label", Label));
            doc.Root.Add(new XAttribute("Datatype", datatype.ToString()));

            for(int i = 0; i < Location.Count(); i++)
            {
                XElement ele = new XElement("Data");
                ele.Add(new XAttribute("Number", i));
                ele.Add(new XAttribute("Location", Location[i]));

                doc.Root.Add(ele);
            }

            doc.Save(Filename);
        }
    }
}
