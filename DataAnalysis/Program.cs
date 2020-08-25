using System;
using System.Threading;
using CC_Library.Predictions;
using System.Diagnostics;

namespace DataAnalysis
{
    class Program
    {
        public static void Write(string wo)
        {
            var time = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            Console.WriteLine(wo);
        }
        public static void holdfortime()
        {
            Thread.Sleep(1400);
        }
        public static int loopcount()
        {
            int loopcount;
            Console.WriteLine("Enter The Number Of Loops To Run.");
            string loops = Console.ReadLine();
            while (!int.TryParse(loops, out loopcount))
            {
                Console.WriteLine("Enter The Number Of Loops To Run.");
                loops = Console.ReadLine();
            }
            return loopcount;
        }
        [STAThread]
        static void Main(string[] args)
        {
            /*
            int loopcount;
            Console.WriteLine("Enter The Number Of Loops To Run.");
            string loops = Console.ReadLine();
            while (!int.TryParse(loops, out loopcount))
            {
                Console.WriteLine("Enter The Number Of Loops To Run.");
                loops = Console.ReadLine();
            };*/
            Datasets.RunPredictions(new WriteToCMDLine(Write), new CC_Library.Predictions.Hold(holdfortime));
        }
    }
}