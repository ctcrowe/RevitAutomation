using System;
using CC_Library;

namespace DataAnalysis
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GetPredictions.Run(WriteAccuracy);
        }
        public static void WriteAccuracy(string s)
        {
            Console.WriteLine(s);
        }
    }
}
