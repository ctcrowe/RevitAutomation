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
        }
        [STAThread]
        static void Main(string[] args)
        {
            /*
            bool enumfound = false;
            while (!enumfound)
            {
                Console.WriteLine("Enter a Datatype");
                string datatype = Console.ReadLine();
                if (Enum.GetNames(typeof(Datatype)).Any(x => x == datatype))
                {
                    enumfound = true;
                    Datatype type = (Datatype)Enum.Parse(typeof(Datatype), datatype);
                    bool finished = false;
                    while (!finished)
                    {
                        Console.WriteLine("Enter a Value");
                        string value = Console.ReadLine();
                        string result = type.FindClosest(value);
                        Console.WriteLine("The result is : " + result);
                        Console.WriteLine("Would you like to continue? y / n");
                        string yesno = Console.ReadLine();
                        if(yesno == "n" || yesno == "N")
                        {
                            finished = true;
                            break;
                        }
                    }
                }
            }
            */
        }
    }
}