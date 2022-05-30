using System;
using System.Linq;
using System.Windows.Forms;
using CC_Library.Datatypes;
using System.IO;

namespace CC_Library.Predictions
{
    public static class PredTest
    {
        public static void TestPredictions(string phrase, string dt, WriteToCMDLine write)
        {
            switch (dt)
            {
                default:
                case "Casework":
                    {
                        var outputs = ObjStyleNetwork.Predict(phrase, typeof(ObjectStyles_Casework), write);
                        var output = outputs.ToList().IndexOf(outputs.Max());
                        try { outputs.WriteArray("Values", write); } catch (Exception e) { e.OutputError(); }
                        write(phrase + " : " + Enum.GetNames(typeof(ObjectStyles_Casework))[output] + " : " + output.ToString());
                        break;
                    }
                case "Masterformat":
                    {
                        var outputs = MasterformatNetwork.Predict(phrase, write);
                        var output = outputs.ToList().IndexOf(outputs.Max());
                        try { outputs.WriteArray("Values", write); } catch (Exception e) { e.OutputError(); }
                        write(phrase + " : Division Number : " + output.ToString());
                        break;
                    }
            }
        }
    }
}
