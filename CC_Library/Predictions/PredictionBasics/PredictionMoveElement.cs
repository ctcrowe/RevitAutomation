using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class PredictionMoveElement
    {
        public static bool Move(this Element ele, double[] adjustment, Element Reference)
        {
            double[] basis = new double[ele.Location.Count()];
            for (int i = 0; i < basis.Count(); i++)
                basis[i] = ele.Location[i];
            for (int i = 0; i < adjustment.Count(); i++)
            {
                if (ele.datatype == Reference.datatype)
                {
                    if (ele.datatype != Datatypes.Datatype.Dictionary)
                    {
                        ele.Location[i] += adjustment[i];
                    }
                    else
                    {
                        if (ele.Location[i] + adjustment[i] <= 1 && ele.Location[i] + adjustment[i] >= -1)
                            ele.Location[i] += adjustment[i];
                        else
                        {
                            if (ele.Location[i] + adjustment[i] >= 1)
                                ele.Location[i] = 1;
                            if (ele.Location[i] + adjustment[i] <= -1)
                                ele.Location[i] = -1;
                        }
                    }
                }
                else
                {
                    if (ele.datatype != Datatypes.Datatype.Dictionary)
                    {
                        ele.Location[i] += adjustment[i];
                    }
                    else
                    {
                        if (ele.Location[i] + adjustment[i] <= 1 &&
                            ele.Location[i] + adjustment[i] >= -1)
                            ele.Location[i] += adjustment[i];
                        else
                        {
                            if (ele.Location[i] + adjustment[i] >= 1)
                                ele.Location[i] = 1;
                            if (ele.Location[i] + adjustment[i] <= -1)
                                ele.Location[i] = -1;
                        }
                    }
                }
            }
            return !ele.Location.SequenceEqual(basis);
        }
        public static void Multiply(this Element ele, double adjustment, Element Reference)
        {
            for (int i = 0; i < Reference.Location.Count(); i++)
            {
                double val = ele.Location[i] * adjustment;
                if (val > 1)
                    val = 1;
                if (val < -1)
                    val = -1;
                ele.Location[i] = val;
            }
        }
        public static string BulkMove(this Dictionary<string, Element>[] Datasets,
            List<double[]> Location,
            List<string>[] Changes,
            double ChangeValue)
        {
            string Mod = "";
            int loccount = 0;
            for (int i = 0; i < Changes.Count(); i++)
            {
                for (int j = 0; j < Changes[i].Count(); j++)
                {
                    double[] values = new double[Location[loccount].Count()];
                    for (int k = 0; k < Location[loccount].Count(); k++)
                    {
                        values[k] = Location[loccount][k] * ChangeValue;
                    }
                    if (Datasets[i][Changes[i][j]].Move(values, Datasets[0].First().Value))
                    {
                        Mod += " " + Changes[i][j];
                    }
                    loccount++;
                }
            }
            return Mod;
        }
        public static void BulkWrite(this Dictionary<string, Element>[] Datasets,
            List<string>[] Changes)
        {
            for(int i = 0; i < Changes.Count(); i++)
            {
                for(int j = 0; j < Changes[i].Count(); j++)
                    Datasets[i][Changes[i][j]].Write();
            }
        }
    }
}
