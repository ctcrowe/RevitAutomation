using System;

namespace Trader
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                TaskScheduler.Instance.ScheduleCycle(.001);

                string line = Console.ReadLine();
                if (line == "x")
                    break;
            }
        }
    }
}
