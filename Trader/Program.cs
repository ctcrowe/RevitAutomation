using System;

namespace Trader
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                _ = TestingClass.Main();

                string line = Console.ReadLine();
                if (line == "x")
                    break;
            }
        }
    }
}
