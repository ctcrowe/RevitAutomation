using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    internal static class XmlResultantDatapoint
    {
        public static double[] ResultantDatapoint(this IEnumerable<XDocument> datum, XDocument RefDatum)
        {
            double[] values = new double[RefDatum.Root.Elements("Data").Count()];
            int valuecount = 0;
            foreach (XDocument d in datum)
            {
                valuecount++;
                for (int i = 0; i < values.Count(); i++)
                {
                    string referencing = RefDatum.Root.Elements("Data").ElementAt(i).Attribute("Referencing").Value;
                    values[i] += double.Parse(d.Root.Elements("Data").
                        Where(x => x.Attribute("Location").Value == referencing).First().
                        Attribute("Value").Value);
                }
            }
            for (int i = 0; i < values.Count(); i++)
                values[i] /= valuecount;
            return values;
        }
    }
}
