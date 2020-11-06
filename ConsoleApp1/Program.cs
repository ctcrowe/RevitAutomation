using System;
using System.Linq;
using System.Threading;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace DataAnalysis
{
    class Program
    {
        public static void Write(string wo)
        {
            Console.WriteLine(wo);
            Thread.Sleep(500);
        }
        [STAThread]
        static void Main(string[] args)
        {
            bool finished = false;
            while (!finished)
            {
                Console.WriteLine("Enter a Phrase");
                string datatype = Console.ReadLine();
                var entries = datatype.GetWords();
                if (entries.Any())
                {
                    string result = "The Words Are : " + entries[0];
                    for (int i = 1; i < entries.Count(); i++)
                    {
                        result += ", " + entries[i];
                    }
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("Error, No Words Found!");
                }
                Console.WriteLine("Would you like to continue? y / n");
                string yesno = Console.ReadLine();
                if (yesno == "n" || yesno == "N")
                {
                    finished = true;
                    break;
                }
            }
        }
    }
}