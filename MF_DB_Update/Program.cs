using System;
using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;
using System.IO;
using System.Diagnostics;

namespace MF_DB_Update
{
    class Program
    {
        public static string Write(string wo)
        {
            var time = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            Console.WriteLine(wo);
            return wo;
        }
        static void Main(string[] args)
        {
        }
    }
}
