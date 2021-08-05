using System;
using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace CC_Library
{
    internal class COutput
    {
        public static void Clear()
        {
            Console.Clear();
        }
        public static void OutputError(this Datatype dt, Sample s, double[] e)
        {
            string f = "Error.txt";
            string filepath = f.GetMyDocs();

            using (StreamWriter writer = new StreamWriter(filepath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
        }
        public static void Update(string epoch, Accuracy acc)
        {
            string[] l = acc.Get();
            try
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, 0);
                var time = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
                epoch += " : " + time + " elapsed";
                Console.Write(epoch);
            }
            catch
            {
                Console.WriteLine(epoch);
            }
            for(int i = 0; i < l.Count(); i++)
            {
                try
                {
                    Console.SetCursorPosition(0, i + 1);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, i + 1);
                    Console.Write(l[i]);
                }
                catch
                {
                    Console.WriteLine(l[i]);
                }
            }
        }
    }
}
