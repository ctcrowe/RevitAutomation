using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class ModAdjustDistance
    {
        public static void AdjustDistance(
            this Dictionary<string, Element>[] DataSets,
            List<Entry> Entries,
            Accuracy accuracy,
            WriteToCMDLine write)
        {
            int runs = 0;
            string[] labels = DataSets[1].Keys.OrderBy(x => new Random().Next()).Take(30).ToArray();
            double[] basis = new double[labels.Count()];
            for(int i = 0; i < basis.Count(); i++)
            {
                basis[i] = 1;
            }
            while(true)
            {
                var Possibilities = PossArrays(basis);
                KeyValuePair<int, double[]>[] Locations = new KeyValuePair<int, double[]>[Possibilities.Count()];
                Parallel.For(0, Possibilities.Count(), j =>
                {
                    var Clone = DataSets.CloneData();
                    for (int k = 0; k < Possibilities[j].Count(); k++)
                    {
                        Clone[1][labels[k]].Multiply(Possibilities[j][k], Clone[0].First().Value);
                        double[] acc = accuracy(Entries, Clone);
                        Locations[j] = new KeyValuePair<int, double[]>(j, acc);
                    }
                });
                var Result = Possibilities.ElementAt(Locations.FindResult().Key);
                bool test = false;
                for(int i = 0; i < Result.Count(); i++)
                {
                    if (Result[i] <= basis[i] - 0.01 && Result[i] >= basis[i] + 0.01)
                    {
                        test = true;
                        break;
                    }
                }
                if (!test)
                    break;
                else
                    basis = Result;
                runs++;
            }
            write("Adjustment Runs : " + runs);
            for (int i = 0; i < labels.Count(); i++)
                DataSets[1][labels[i]].Multiply(basis[i], DataSets[0].First().Value);
        }
        private static List<double[]> PossArrays(this double[] basis)
        {
            List<double[]> Possibilities = new List<double[]>();
            Possibilities.Add(basis);
            double[] nextarray = new double[basis.Count()];
            for (int i = 0; i < nextarray.Count(); i++)
            {
                nextarray[i] = basis[i] * 0.5;
            }
            Possibilities.Add(nextarray);
            while(true)
            {
                double[] newarray = new double[nextarray.Count()];
                for(int i = 0; i < newarray.Count(); i++)
                {
                    newarray[i] = nextarray[i];
                }
                for(int i = 0; i < newarray.Count() - 1; i++)
                {
                    if (nextarray[i + 1] >= basis[i] * 1.1)
                        if (nextarray[i] >= basis[i] * 1.1)
                            newarray[i] = basis[i] * 0.5;
                        else
                            newarray[i] += basis[i] * 0.5;
                }

                if (nextarray[newarray.Count() - 1] < basis[basis.Count() - 1] * 1.1)
                    newarray[newarray.Count() - 1] += basis[basis.Count() - 1] * 0.5;
                else
                    newarray[newarray.Count() - 1] = basis[basis.Count() - 1] * 0.5;

                Possibilities.Add(newarray);
                bool check = false;
                Parallel.For(0, basis.Count(), i
                    =>
                {
                    if (basis[i] * 1.1 >= newarray[i])
                        check = true;
                });
                if (check)
                    for (int i = 0; i < nextarray.Count(); i++)
                    {
                        nextarray[i] = newarray[i];
                    }
                else
                    break;
            }
            return Possibilities;
        }
    }
}
