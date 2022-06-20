using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CC_Library.Predictions
{
    public static class CMD_CreateSample
    {
        public static void CreateEmbed(this Type t, string input, string output, string SecondaryInput = null)
        {
            string baseinput = input;
            if (SecondaryInput != null)
                baseinput += "_" + SecondaryInput;
            var dir = "NetworkSamples".GetMyDocs();
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var subdir = dir + "\\" + t.ToString().Split('.').Last();
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);

            var fn = subdir + "\\" + t.ToString().Split('.').Last() + "_" + baseinput + ".txt";
            List<string> Lines = new List<string>();

            if (SecondaryInput != null)
                Lines.Add(t.ToString().Split('.').Last() + "," + input + "," + SecondaryInput + "," + output);
            else
                Lines.Add(t.ToString().Split('.').Last() + "," + input + "," + output);
            File.WriteAllLines(fn, Lines);
        }
    }
}