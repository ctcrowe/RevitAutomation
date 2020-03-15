using System;
using CC_Library;
using CC_Library.Predictions;

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
            DataFile.Masterformat.Create(new CMDGetMyDocs.WriteOutput(Write));
            while (true) { }
        }
    }
}
