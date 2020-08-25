using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CC_Library.Datatypes;

using System.Xml.Linq;

/// <summary>
/// This should take the array from any datatype
/// and pair it with the best possible spot in the Dictionary Data Array.
/// Note: This will need a special accuracy calculation entered for each datatype.
/// </summary>
/*
namespace CC_Library.Predictions
{
    internal static class XmlCorrelateArrays
    {
        public static void CorrelateArrays
            (this List<XDocument> Dataset,
            List<XDocument> CompSet,
            Dictionary<string, string[]> EntrySet,
            XmlAccuracy CalcAcc, WriteToCMDLine write)
        {
            int DataSize = Dataset.First().Root.Elements("Data").Count();
            write("Correlation Data Size " + DataSize);
            int DictSize = CompSet.First().Root.Elements("Data").Count();

            int[] CorrelatedLocation = new int[DataSize];
            for (int i = 0; i < DataSize; i++)
            {
                CorrelatedLocation[i] = i;
            }

            double BaseAccuracy = 0;
            List<int[]> PosArrays = PossibleArrays(DataSize, DictSize);

            Parallel.For(0, PosArrays.Count(), i =>
            {
                var CopyData = Datatype.Masterformat.GetElements();
                var CopyComp = Datatype.Dictionary.GetElements();

                foreach (var doc in CopyData)
                {
                    doc.SetXMLCorrelation(PosArrays[i]);
                }
                double Accuracy = CalcAcc(CopyData, CopyComp, EntrySet);

                if (Accuracy > BaseAccuracy)
                {
                    BaseAccuracy = Accuracy;
                    CorrelatedLocation = PosArrays[i];
                }
            });
            for (int i = 0; i < DataSize; i++)
            {
                write("Correlation at " + i + " : " + CorrelatedLocation[i]);
            }
            write("Base Accuracy is " + BaseAccuracy);
            foreach(var doc in Dataset)
            {
                doc.SetXMLCorrelation(CorrelatedLocation);
            }
        }
        public static void SetXMLCorrelation(this XDocument doc, int[] PairedLocation)
        {
            for (int i = 0; i < PairedLocation.Count(); i++)
            {
                doc.Root.Elements().Where(x => int.Parse(x.Attribute("Location").Value) == i).First()
                    .Attribute("Referencing").Value = PairedLocation[i].ToString();
            }
        }
        private static List<int[]> PossibleArrays(int ArrayLength, int ComparativeArrayCount)
        {
            List<int[]> posarrays = new List<int[]>();
            int[] firstarray = new int[ArrayLength];
            int[] nextarray = new int[ArrayLength];

            int ComparativeLength = ComparativeArrayCount - ArrayLength;

            if (ComparativeLength > 0)
            {
                for (int i = 0; i < firstarray.Count(); i++)
                {
                    firstarray[i] = i;
                    nextarray[i] = i;
                }
                posarrays.Add(firstarray);

                while (true)
                {
                    int[] newarray = new int[ArrayLength];
                    for (int i = 0; i < newarray.Count(); i++)
                    {
                        newarray[i] = nextarray[i];
                    }

                    if (nextarray[1] == (ComparativeLength + 1))
                        newarray[0] = nextarray[0] + 1;

                    for (int i = 1; i < ArrayLength - 1; i++)
                    {
                        if (nextarray[i + 1] == (ComparativeLength + i + 1))
                        {
                            if (nextarray[i] == (ComparativeLength + i))
                                newarray[i] = newarray[i - 1] + 1;
                            else
                                newarray[i] = nextarray[i] + 1;
                        }
                    }

                    if (nextarray[ArrayLength - 1] < (ComparativeArrayCount - 1))
                        newarray[ArrayLength - 1] = nextarray[ArrayLength - 1] + 1;
                    else
                        newarray[ArrayLength - 1] = newarray[ArrayLength - 2] + 1;

                    posarrays.Add(newarray);

                    if (newarray[0] == ComparativeLength)
                        break;
                    else
                        for (int i = 0; i < nextarray.Count(); i++)
                        {
                            nextarray[i] = newarray[i];
                        }
                }
            }
            return posarrays;
        }
    }
}*/