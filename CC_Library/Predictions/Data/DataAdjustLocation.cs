using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class AdjustDataLocation
    {
        public static void AdjustLocation
            ( this KeyValuePair<string, Data> datapoint,
            double[] values,
            double adjustment,
            bool positive
            )
        {
            double Length = values.VectorLength();
            if (!positive)
            {
                double pi = Math.PI / 2;
                Length = -1 * Math.Cos(Length * pi / 2 * pi);
            }
            for (int i = 0; i < datapoint.Value.Location.Count(); i++)
            {
                double LengthI = datapoint.Value.Location[i] + (values.NormalizeVector()[i] * Length * adjustment);
                if (LengthI <= 1 && LengthI >= -1)
                    datapoint.Value.Location[i] = LengthI;
                else
                {
                    if (LengthI >= 1)
                        datapoint.Value.Location[i] = 1;
                    if (LengthI <= -1)
                        datapoint.Value.Location[i] = -1;
                }
            }
        }
        public static void AdjustLocation
            (this KeyValuePair<string, Data> datapoint,
            double[] values,
            double adjustment)
        {
            double DataLength = datapoint.Value.Location.VectorLength();
            double[] ChangeDirection = values.DirectionBetween(datapoint.Value.Location);
            ChangeDirection = ChangeDirection.NormalizeVector();

            for (int i = 0; i < datapoint.Value.Location.Count(); i++)
            {
                if (ChangeDirection[i] > 0)
                {
                    double change = ChangeDirection[i] * adjustment;

                    datapoint.Value.Location[i] += change;
                }
            }
            datapoint.Value.Location = datapoint.Value.Location.NormalizeVector();
            for (int i = 0; i < datapoint.Value.Location.Count(); i++)
            {
                datapoint.Value.Location[i] *= DataLength;
            }
        }
        public static void Move
            (this KeyValuePair<string, Data> Datum
            , double[] LengthVector)
        {
            for (int i = 0; i < Datum.Value.Location.Count(); i++)
            {
                if (LengthVector[i] + Datum.Value.Location[i] <= 1 && LengthVector[i] + Datum.Value.Location[i] >= -1)
                    Datum.Value.Location[i] += LengthVector[i];
                else
                {
                    if (LengthVector[i] + Datum.Value.Location[i] >= 1)
                        Datum.Value.Location[i] = 1;
                    if (LengthVector[i] + Datum.Value.Location[i] <= -1)
                        Datum.Value.Location[i] = -1;
                }
            }
        }
        public static void AdjustDistance
            (this KeyValuePair<string, Data> Datum
            ,double[] LengthVector)
        {
            for (int i = 0; i < Datum.Value.Location.Count(); i++)
            {
                if (LengthVector[i] <= 1 && LengthVector[i] >= -1)
                    Datum.Value.Location[i] = LengthVector[i];
                else
                {
                    if (LengthVector[i] >= 1)
                        Datum.Value.Location[i] = 1;
                    if (LengthVector[i] <= -1)
                        Datum.Value.Location[i] = -1;
                }
            }
        }
    }
}