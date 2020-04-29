using System;
using CC_Library;

namespace DataAnalysis
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string Dataset = directory + "\\CuratedData";
            //GetPredictions.Run("MASTERFORMAT", Dataset, "Name", "MFSection", WriteAccuracy);
        }
        public static void WriteAccuracy(string s)
        {
            Console.WriteLine(s);
        }
    }
}
