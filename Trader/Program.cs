using System;

namespace Trader
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                TaskScheduler.Instance.ScheduleCycle(17, 22, .01);

                string line = Console.ReadLine();
                if (line == "x")
                    break;
            }
        }
    }
}
