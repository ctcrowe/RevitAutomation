using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace DataAnalysis
{
    class ObjectTimeVariance
    {
        public static void RunComparison(string folder, string file)
        {
            string[] files = Directory.GetFiles(folder);
            var Data = new Dictionary<string, Dictionary<string, int>>();

            foreach (string f in files)
            {

            }
        }
    }
}
