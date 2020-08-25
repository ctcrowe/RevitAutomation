using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class XMLArrayPossibilities
    {
        public static List<List<double[]>> PossibleArrays(this List<XDocument> dataset, int NumberToChange)
        {
            List<List<double[]>> ChangeArrays = new List<List<double[]>>();

            double[] nextarray = new double[NumberToChange * dataset.First().Root.Elements("Data").Count()];
            for (int i = 0; i < nextarray.Count(); i++)
            {
                nextarray[i] = -1;
            }
            while (true)
            {
                double[] newarray = new double[nextarray.Count()];

                for (int i = 0; i < newarray.Count(); i++)
                {
                    newarray[i] = nextarray[i];
                }

                for (int i = 0; i < newarray.Count() - 1; i++)
                {
                    if (nextarray[i + 1] >= 1)
                    {
                        if (nextarray[i] >= 1)
                            newarray[i] = -1;
                        else
                            newarray[i] += 0.5;
                    }
                }

                if (nextarray[newarray.Count() - 1] < 1)
                    newarray[newarray.Count() - 1] += 0.5;
                else
                    newarray[newarray.Count() - 1] = -1;

                List<double[]> finlist = new List<double[]>();
                for (int i = 0; i < newarray.Count(); i += dataset.First().Root.Elements("Data").Count())
                {
                    double[] finarray = new double[dataset.First().Root.Elements("Data").Count()];
                    for (int j = 0; j < dataset.First().Root.Elements("Data").Count(); j++)
                    {
                        finarray[j] = newarray[i + j];
                    }
                    finlist.Add(finarray);
                }
                ChangeArrays.Add(finlist);

                if (newarray.Any(x => x < 1))
                    for (int i = 0; i < nextarray.Count(); i++)
                    {
                        nextarray[i] = newarray[i];
                    }
                else
                    break;
            }

            return ChangeArrays;
        }
    }
}
