using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class PredictionMaster
    {
        public static int PredictSingle(this Datatype dt, string s = null, double[] other = null, double[] img = null)
        {
            Sample entry = new Sample(dt);
            if(s != null) { entry.TextInput = s; }
            if(other != null) { entry.ValInput = other; }
            if (img != null) { entry.ImgInput = img; }

            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(x => type.IsAssignableFrom(x)).ToList();
            int prediction = -1;
            for(int i = 0; i < NetTypes.Count(); i++)
            {
                INetworkPredUpdater Network = NetTypes[i] as INetworkPredUpdater;
                if(Network.datatype == dt)
                {
                    var output = Network.Predict(entry);
                    prediction = output.ToList().IndexOf(output.Max());
                    break;
                }
            }
            return prediction;
        }
        public static void PropogateSingle(this Datatype dt, int correct, WriteToCMDLine write, string s = null, double[] other = null, double[] img = null)
        {
            Sample entry = new Sample(dt);
            if(s != null) { entry.TextInput = s; }
            if(other != null) { entry.ValInput = other; }
            if (img != null) { entry.ImgInput = img; }

            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(x => type.IsAssignableFrom(x)).ToList();
            for(int i = 0; i < NetTypes.Count(); i++)
            {
                INetworkPredUpdater Network = NetTypes[i] as INetworkPredUpdater;
                if(Network.datatype == dt)
                {
                    entry.DesiredOutput = new double[Network.Network.Layers.Last().Biases.Count()];
                    entry.DesiredOutput[correct] = 1;
                    Network.Propogate(entry, write);
                    break;
                }
            }
        }
    }
}
