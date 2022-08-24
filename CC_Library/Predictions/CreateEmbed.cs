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
            string baseinput = input.Replace(',', '_').Replace('.', '_');
            if (SecondaryInput != null)
                baseinput += "_" + SecondaryInput.Replace(',', '_').Replace('.', '_');
            var dir = "NetworkSamples".GetMyDocs();
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fn = dir + "\\" + t.ToString().Split('.').Last() + ".txt";
            List<string> Lines = new List<string>();

            if (SecondaryInput != null)
                Lines.Add(input.Replace(',', '_') + "," + SecondaryInput.Replace(',', '_') + "," + output);
            else
                Lines.Add(input.Replace(',', '_') + "," + output);
            File.AppendAllLines(fn, Lines);
        }
    }
}
