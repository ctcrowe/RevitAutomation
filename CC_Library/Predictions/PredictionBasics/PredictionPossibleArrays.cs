using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class PredictionPossibleArrays
    {
        public static List<double[]> ZeroArray(this Datatype datatype, int NumberToChange)
        {
            List<double[]> ChangeArrays = new List<double[]>();
            int count = datatype.Count();
            for(int i = 0; i < NumberToChange; i++)
            {
                double[] values = new double[count];
                ChangeArrays.Add(values);
            }
            return ChangeArrays;
        }
        public static List<List<double[]>> PossibleArrays(this Datatype datatype, int NumberToChange)
        {
            List<List<double[]>> ChangeArrays = new List<List<double[]>>();

            ChangeArrays.Add(datatype.ZeroArray(NumberToChange));
            double[] nextarray = new double[NumberToChange * datatype.Count()];
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
                for (int i = 0; i < newarray.Count(); i += datatype.Count())
                {
                    double[] finarray = new double[datatype.Count()];
                    for (int j = 0; j < datatype.Count(); j++)
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
