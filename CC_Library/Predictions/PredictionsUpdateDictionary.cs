using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class PredictionsUpdateDictionary
    {
        public static void Update(this Dictionary<string, Data> BaseDict, KeyValuePair<string, Data> Datapoint)
        {

            if (!BaseDict.Keys.Any(x => x == Datapoint.Key))
            {
                double[] values = new double[((int)Datatype.Dictionary / 1000) % 10];
                for (int i = 0; i < Datapoint.Value.RefPoints.Count(); i++)
                {
                    values[Datapoint.Value.RefPoints[i]] = Datapoint.Value.Location[i];
                }
                Random rand = new Random();
                for (int i = 0; i < values.Count(); i++)
                {
                    if (!Datapoint.Value.RefPoints.Any(x => x == i))
                    {
                        values[i] = rand.NextDouble();
                    }
                }
            }
            else
            {
                for (int i = 0; i < Datapoint.Value.RefPoints.Count(); i++)
                {
                    if (BaseDict[Datapoint.Key].Location.Count() > Datapoint.Value.RefPoints[i])
                        BaseDict[Datapoint.Key].Location[Datapoint.Value.RefPoints[i]] = Datapoint.Value.Location[i];
                    else
                    {
                        double[] values = new double[((int)Datatype.Dictionary / 1000) % 10];
                        for (int j = 0; j < Datapoint.Value.RefPoints.Count(); j++)
                        {
                            values[Datapoint.Value.RefPoints[j]] = Datapoint.Value.Location[j];
                        }
                        Random rand = new Random();
                        for (int j = 0; j < values.Count(); j++)
                        {
                            if (!Datapoint.Value.RefPoints.Any(x => x == j))
                            {
                                values[j] = rand.NextDouble();
                            }
                        }
                    }
                }
            }
        }
        public static void Update(this Dictionary<string, Data> BaseDict, Dictionary<string, Data> Dataset)
        {
            foreach (KeyValuePair<string, Data> Datapoint in Dataset)
            {
                if (!BaseDict.Keys.Any(x => x == Datapoint.Key))
                {
                    double[] values = new double[((int)Datatype.Dictionary / 1000) % 10];
                    for (int i = 0; i < Datapoint.Value.RefPoints.Count(); i++)
                    {
                        values[Datapoint.Value.RefPoints[i]] = Datapoint.Value.Location[i];
                    }
                    Random rand = new Random();
                    for (int i = 0; i < values.Count(); i++)
                    {
                        if (!Datapoint.Value.RefPoints.Any(x => x == i))
                        {
                            values[i] = rand.NextDouble();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Datapoint.Value.RefPoints.Count(); i++)
                    {
                        if (BaseDict[Datapoint.Key].Location.Count() > Datapoint.Value.RefPoints[i])
                            BaseDict[Datapoint.Key].Location[Datapoint.Value.RefPoints[i]] = Datapoint.Value.Location[i];
                        else
                        {
                            double[] values = new double[((int)Datatype.Dictionary / 1000) % 10];
                            for (int j = 0; j < Datapoint.Value.RefPoints.Count(); j++)
                            {
                                values[Datapoint.Value.RefPoints[j]] = Datapoint.Value.Location[j];
                            }
                            Random rand = new Random();
                            for (int j = 0; j < values.Count(); j++)
                            {
                                if (!Datapoint.Value.RefPoints.Any(x => x == j))
                                {
                                    values[j] = rand.NextDouble();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
