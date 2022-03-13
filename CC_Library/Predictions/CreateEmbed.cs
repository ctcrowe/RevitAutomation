using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public static class CMD_CreateEmbed
    {
        public static List<string> CreateEmbed(NeuralNetwork net, string fn)
        {
            var Lines = new List<string>();
            Lines.Add("using System;");
            Lines.Add("using System.Collections.Generic;");
            Lines.Add("using System.Linq;");
            Lines.Add("using System.Threading.Tasks;");
            Lines.Add("using CC_Library.Datatypes;");
            Lines.Add("");
            Lines.Add("namespace CC_Library.Predictions");
            Lines.Add("{");

            Lines.Add("public static class " + fn);
            Lines.Add("{");

            Lines.Add("public static double[] Output(double[] outputs");
            Lines.Add("{");

            for(int i = 0; i < net.Layers.Count(); i++)
            {
                string s = "";
                Lines.Add(s);
                Lines.Add("");
                Lines.Add("");
            }
            Lines.Add("return outputs;");

            Lines.Add("}");
            Lines.Add("}");
            Lines.Add("}");
            return Lines;
        }
    }
}
