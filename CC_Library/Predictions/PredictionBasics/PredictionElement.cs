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
        public int[] Referencing;
        public int total;
        public int correct;
        public double accuracy;
        public Datatype datatype;

        public Element(Datatype dt, string Name)
        {
            this.Label = Name;
            this.datatype = dt;
            Random r = new Random();

            double[] Loc = new double[dt.Count()];
            int[] Ref = new int[dt.Count()];
            for (int i = 0; i < dt.Count(); i++)
            {
                double swap = r.NextDouble();
                double v = r.NextDouble();
                if (swap > 0.50)
                    Loc[i] = v;
                else
                    Loc[i] = -v;
                Ref[i] = i;
            }

            this.Location = Loc;
            this.Referencing = Ref;
            this.total = 0;
            this.correct = 0;
            this.accuracy = 0;
        }
        public Element(XDocument doc)
        {
            this.Label = doc.Root.Attribute("Label").Value;
            this.datatype = (Datatype)Enum.Parse(typeof(Datatype), doc.Root.Attribute("Datatype").Value);
            double[] Loc = new double[datatype.Count()];
            int[] Ref = new int[Loc.Count()];
            for (int i = 0; i < datatype.Count(); i++)
            {
                if (doc.Root.Elements("Data").Any(x => x.Attribute("Number").Value == i.ToString()))
                {
                    XElement ele = doc.Root.Elements("Data").Where(x => int.Parse(x.Attribute("Number").Value) == i).First();
                    Ref[i] = int.Parse(ele.Attribute("Referencing").Value);
                    Loc[i] = double.Parse(ele.Attribute("Location").Value);
                }
                else
                {
                    if (i == 0)
                        Ref[i] = 0;
                    else
                        Ref[i] = Ref[i - 1] + 1;
                    Random r = new Random();
                    Loc[i] = r.NextDouble();
                }
            }
            this.Location = Loc;
            this.Referencing = Ref;
            this.total = 0;
            this.correct = 0;
            this.accuracy = 0;
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
                ele.Add(new XAttribute("Referencing", Referencing[i]));
                ele.Add(new XAttribute("Location", Location[i]));

                doc.Root.Add(ele);
            }

            doc.Save(Filename);
        }
    }
}
