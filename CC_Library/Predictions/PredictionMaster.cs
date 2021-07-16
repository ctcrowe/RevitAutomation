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
            var NetType = a.GetTypes().Where(x => type.IsAssignableFrom(x)).Where(y => (y as INetworkPredUpdater).datatype == dt).First();
            INetworkPredUpdater Network = NetType as INetworkPredUpdater;
            var output = Network.Predict(entry);
            return output.ToList().IndexOf(output.Max());
        }
        public static void PropogateSingle(this Datatype dt, int correct, WriteToCMDLine write, string s = null, double[] other = null, double[] img = null)
        {
            Sample entry = new Sample(dt);
            if(s != null) { entry.TextInput = s; }
            if(other != null) { entry.ValInput = other; }
            if (img != null) { entry.ImgInput = img; }

            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetType = a.GetTypes().Where(x => type.IsAssignableFrom(x)).Where(y => (y as INetworkPredUpdater).datatype == dt).First();
            INetworkPredUpdater Network = NetType as INetworkPredUpdater;
            entry.DesiredOutput = new double[Network.Network.Layers.Last().Biases.Count()];
            entry.DesiredOutput[correct] = 1;
            Network.Propogate(entry, write);
        }
    }
}
