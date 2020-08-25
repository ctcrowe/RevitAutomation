using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class XMLDistance
    {
        public static double CalcDistance(this XDocument point1, XDocument point2)
        {
            double val = 0;
            foreach (XElement data in point1.Root.Elements("Data"))
            {
                int referencing = int.Parse(data.Attribute("Referencing").Value);
                if (point2.Root.Elements("Data").Any(x => int.Parse(x.Attribute("Location").Value) == referencing))
                {
                    double a = double.Parse(data.Attribute("Value").Value);
                    double b = double.Parse(point2.Root.Elements("Data").
                        Where(x => int.Parse(x.Attribute("Location").Value) == referencing).First().
                        Attribute("Value").Value);
                    double c = Math.Pow(b - a, 2);
                    val += c;
                }
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
        public static double CalcDistance(this XDocument point1, double[] point2)
        {
            double val = 0;
            for (int i = 0; i < point1.Root.Elements("Data").Count(); i++)
            {
                if (point2.Count() > i)
                {
                    double a = double.Parse(point1.Root.Elements("Data").ElementAt(i).Attribute("Value").Value);
                    double b = Math.Pow(point2[i] - a, 2);
                    val += b;
                }
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
    }
}
