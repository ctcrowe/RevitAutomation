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
            var fn = dir + "\\" + t.ToString().Split('.').Last() + "_" + input + "_" + output + ".txt";
            List<string> Lines = new List<string>();

            Lines.Add(t.ToString().Split('.').Last() + "," + input + "," + output);
            File.WriteAllLines(fn, Lines);
        }
    }
}