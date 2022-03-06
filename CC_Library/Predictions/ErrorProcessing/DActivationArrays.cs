using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class DActivationArrays
    {
        //ldv = ValMem.Layers[i].DActivation(ldv, am.LocationOutputs[j][i + 1].GetRank(1));
        public static void ActivationError(this Exception ex, int layerno, double[] dvalues, double[] outputs)
        {
            string f = "Error.txt";
            string filepath = f.GetMyDocs();

            using (StreamWriter writer = new StreamWriter(filepath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();
                writer.WriteLine("Layer Number : " + layerno);
                writer.WriteLine("DValue Count : " + dvalues.Count());
                writer.WriteLine("Output Count : " + outputs.Count());

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
}
