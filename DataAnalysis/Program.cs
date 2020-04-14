using System;
using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace DataAnalysis
{
    class Program
    {
        public static void Write(string wo)
        {
            Console.WriteLine(wo);
        }
        [STAThread]
        static void Main(string[] args)
        {
            Datasets.RunPredictions();
            while (true) { }
        }
    }
}
