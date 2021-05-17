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
            if(Enum.GetNames(typeof(Datatype)).Any(x => dt == x))
            {
                Datatype dtype = (Datatype)Enum.Parse(typeof(Datatype), dt);
                switch(dtype)
                {
                    default:
                    case Datatype.Masterformat:
                        var output = MasterformatNetwork.Predict(phrase).ToString();
                        write(phrase + " : Division Number : " + output);
                        break;
                }
            }
        }
    }
}