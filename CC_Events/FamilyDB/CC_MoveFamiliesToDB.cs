using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Revit.UI;

namespace CC_Events
{
    internal class MoveFamToDatabase
    {
        public static void Run(string input, string output)
        {
            string[] files = Directory.GetFiles(input);
            foreach(string file in files)
            {
                if(!file.Split('\\').Last().Contains('_'))
                {
                    if (File.Exists(output + file.Split('\\').Last()))
                        File.Delete(output + file.Split('\\').Last());
                    File.Move(file, output + file.Split('\\').Last());
                }
            }
        }
    }
}