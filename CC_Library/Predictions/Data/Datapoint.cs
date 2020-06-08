using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal class Data
    {
        public string Label { get; set; }
        public double[] Location { get; set; }
        public double[] PositiveDirection { get; set; }
        public double[] NegativeDirection { get; set; }
        public int[] RefPoints { get; set; }
        public Datatype Datatype { get; }

        public Data(Datatype datatype, string label, Random r)
        {
            double[] values = new double[datatype.Count()];
            for (int i = 0; i < datatype.Count(); i++)
            {
                double swap = r.NextDouble();
                double v = r.NextDouble();
                if (swap > 0.50)
                    values[i] = v;
                else
                    values[i] = -v;
            }

            this.Label = label;
            this.Datatype = datatype;
            this.Location = values;
            this.PositiveDirection = new double[datatype.Count()];
            this.NegativeDirection = new double[datatype.Count()];
            this.RefPoints = new int[datatype.Count()];
        }
        public Data(Datatype type, XElement ele)
        {
            this.Datatype = type;
            double[] Values = new double[type.Count()];
            int[] rpoints = new int[type.Count()];
            if (ele.Attribute("Label") != null)
                this.Label = ele.Attribute("Label").Value;
            else
                this.Label = "Null";
            for (int i = 0; i < Values.Count(); i++)
            {
                List<XElement> dps = ele.Elements("Data").Where(x => x.Attribute("Location").Value == i.ToString()).ToList();
                if (dps.Any())
                {
                    Values[i] = double.Parse(dps.First().Attribute("Value").Value);
                    int RefPoint = -1;
                    if (int.TryParse(dps.First().Attribute("Referencing").Value, out RefPoint))
                        rpoints[i] = RefPoint;
                }
                else
                {
                    Random rand = new Random();
                    double swap = rand.NextDouble();
                    double v = rand.NextDouble();
                    if (swap > 0.50)
                        Values[i] = v;
                    else
                        Values[i] = -v;
                }
            }
            this.Location = Values;
            this.PositiveDirection = new double[type.Count()];
            this.NegativeDirection = new double[type.Count()];
            this.RefPoints = rpoints;

        }
        public Data(Datatype datatype, string label, double[] values)
        {
            this.Label = label;
            this.Datatype = datatype;
            this.PositiveDirection = new double[datatype.Count()];
            this.NegativeDirection = new double[datatype.Count()];
            if (datatype.Count() != values.Count())
            {
                double[] newvalues = new double[datatype.Count()];
                if (datatype.Count() > values.Count())
                {
                    Random random = new Random();
                    for(int i = 0; i < values.Count(); i++)
                    {
                        newvalues[i] = values[i];
                    }
                    for(int i = values.Count(); i < datatype.Count(); i++)
                    {
                        double swap = random.NextDouble();
                        double v = random.NextDouble();
                        if (swap > 0.50)
                            values[i] = v;
                        else
                            values[i] = -v;
                    }
                    this.Location = newvalues;
                }
                else
                {
                    for(int i = 0; i < datatype.Count(); i++)
                    {
                        newvalues[i] = values[i];
                    }
                    this.Location = newvalues;
                }
            }
            else
                this.Location = values;
        }
        public Data(Datatype datatype, string label, double[] values, int[] refpoints)
        {
            this.Label = label;
            this.Datatype = datatype;
            this.Location = values;
            this.PositiveDirection = new double[datatype.Count()];
            this.NegativeDirection = new double[datatype.Count()];
            this.RefPoints = refpoints;
        }
        public XElement Write()
        {
            XElement ele = new XElement("DataPoint");
            ele.Add(new XAttribute("Label", this.Label));
            for (int i = 0; i < this.Location.Count(); i++)
            {
                XElement dp = new XElement("Data");
                dp.Add(new XAttribute("Location", i));
                dp.Add(new XAttribute("Value", this.Location[i]));
                dp.Add(new XAttribute("Referencing", this.RefPoints[i]));

                ele.Add(dp);
            }
            return ele;
        }
    }
    internal static class CloneData
    {
        public static Data Clone(this Data data)
        {
            double[] values = new double[data.Location.Count()];
            int[] refpoints = new int[data.Location.Count()];
            double[] pos = new double[data.Location.Count()];
            double[] neg = new double[data.Location.Count()];

            for (int i = 0; i < values.Count(); i++)
            {
                refpoints[i] = data.RefPoints[i];
                values[i] = data.Location[i];
                neg[i] = data.NegativeDirection[i];
                pos[i] = data.PositiveDirection[i];
            }
            Data newdata = new Data(data.Datatype, data.Label, values, refpoints);
            newdata.PositiveDirection = pos;
            newdata.NegativeDirection = neg;
            return newdata;
        }
        public static Dictionary<string, Data> Clone(this Dictionary<string, Data> Dataset)
        {
            Dictionary<string, Data> clone = new Dictionary<string, Data>();
            foreach(var data in Dataset)
            {
                clone.Add(data.Key, data.Value.Clone());
            }
            return clone;
        }
    }
}