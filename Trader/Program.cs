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
                //TaskScheduler.Instance.ScheduleCycle(07, 39, .1);

                string line = Console.ReadLine();
                if (line == "x")
                    break;
            }
        }
    }
}
