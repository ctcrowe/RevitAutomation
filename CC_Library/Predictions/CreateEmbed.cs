using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CC_Library.Predictions
{
    public static class CMD_CreateSample
    {
        public static void CreateEmbed(this Type t, string input, string output)
        {
            var dir = "NetworkSamples".GetMyDocs();
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fn = dir + "\\" + t.ToString().Split('.').Last() + ".txt";
            List<string> Lines = new List<string>();

            Lines.Add(input + "," + output);
            File.AppendAllLines(fn, Lines);
        }
    }
}
