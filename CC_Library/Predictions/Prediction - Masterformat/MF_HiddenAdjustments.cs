using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class MF_HiddenAdjustments
    {
        public static double[] HiddenAdjustment
            (this double[] Point,
            Dictionary<string, Element> HiddenA,
            Dictionary<string, Element> HiddenB)
        {
            List<double[]> Loc1 = new List<double[]>();
            List<double[]> Loc2 = new List<double[]>();
            for(int a = 0; a < 8; a++)
            {
                double[] HiddenI = new double[Point.Count()];
                for(int b = 0; b < Point.Count(); b++)
                {
                    for(int c = 0; c < Point.Count(); c++)
                    {
                        HiddenI[b] += Point[b] * HiddenA["Masterformat_Weights_A" + a].Location[c];
                    }
                    HiddenI[b] += HiddenA["Masterformat_Biases_A" + a].Location[b];
                }
                Loc1.Add(HiddenI);
            }
            for (int i = 0; i < Loc1.Count(); i++)
            {
                for (int a = 0; a < 8; a++)
                {
                    double[] HiddenI = new double[Loc1[i].Count()];
                    for (int b = 0; b < Point.Count(); b++)
                    {
                        for (int c = 0; c < Point.Count(); c++)
                        {
                            HiddenI[b] += Loc1[i][b] * HiddenB["Masterformat_Weights_B" + a].Location[c];
                        }
                        HiddenI[b] += HiddenB["Masterformat_Biases_B" + a].Location[b];
                    }
                    Loc2.Add(HiddenI);
                }
            }
            double[] loc = new double[Point.Count()];
            for(int i = 0; i < loc.Count(); i++)
            {
                for(int j = 0; j < Loc2.Count(); j++)
                {
                    loc[i] += Loc2[j][i];
                }
            }
            return loc;
        }
    }
}