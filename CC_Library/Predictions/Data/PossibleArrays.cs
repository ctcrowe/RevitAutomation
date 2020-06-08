using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class ArrayPossibilities
    {
        public static List<double[]> PossibleArrays(this Dictionary<string, Data> dataset)
        {
            int ArrayCount = dataset.Count();
            List<double[]> PossibleArray = new List<double[]>();

            double[] PrevArray = new double[ArrayCount];
            double[] newarray = new double[ArrayCount];

            while (true)
            {
                for (int i = 0; i < PrevArray.Count(); i++)
                {

                    newarray[i] = PrevArray[i];
                }

                for (int i = 0; i < PrevArray.Count() - 1; i++)
                {
                    if (PrevArray[i + 1] == 0.5)
                    {
                        if (newarray[i] == 0.5)
                            newarray[i] = 0;
                        else
                            newarray[i] += 0.25;
                    }
                }

                if (PrevArray[PrevArray.Count() - 1] != 0.5)
                    newarray[PrevArray.Count() - 1] += 0.25;
                else
                    newarray[PrevArray.Count() - 1] = 0;

                PossibleArray.Add(newarray);

                if (!newarray.Any(x => x != 0.5))
                    break;
            }
            return PossibleArray;
        }
    }
}
