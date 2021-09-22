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
        public static int PredictSingle(this Datatype dt, string s, string s2 = null, double[] other = null, double[] img = null)
        {
            Sample entry = new Sample(dt);
            entry.TextInput = s;
            if(s2 != null) { entry.SecondaryText = s2; }
            if(other != null) { entry.ValInput = other; }
            if (img != null) { entry.ImgInput = img; }

            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(y => !y.IsInterface).Where(x => type.IsAssignableFrom(x)).ToList();
            int prediction = -1;
            for(int i = 0; i < NetTypes.Count(); i++)
            {
                var Network = (INetworkPredUpdater)Activator.CreateInstance(NetTypes[i], entry);
                if (Network.datatype == dt)
                {
                    var output = Network.Predict(entry);
                    prediction = output.ToList().IndexOf(output.Max());
                    break;
                }
            }
            return prediction;
        }
        public static void PropogateSingle(this Datatype dt, int correct, WriteToCMDLine write, string s, string s2 = null, double[] other = null, double[] img = null)
        {
            Sample entry = new Sample(dt);
            entry.TextInput = s;
            if(s2 != null) { entry.SecondaryText = s2; }
            if(other != null) { entry.ValInput = other; }
            if (img != null) { entry.ImgInput = img; }

            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(y => !y.IsInterface).Where(x => type.IsAssignableFrom(x)).ToList();
            for (int i = 0; i < NetTypes.Count(); i++)
            {
                var Network = (INetworkPredUpdater)Activator.CreateInstance(NetTypes[i], entry);
                if (Network.datatype == dt)
                {
                    entry.DesiredOutput = new double[Network.Network.Layers.Last().Biases.Count()];
                    entry.DesiredOutput[correct] = 1;
                    Network.Propogate(entry, write);
                    break;
                }
            }
        }
        public static void PropogateSingle(this Sample entry, WriteToCMDLine write)
        {
            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(y => !y.IsInterface).Where(x => type.IsAssignableFrom(x)).ToList();
            for (int i = 0; i < NetTypes.Count(); i++)
            {
                var Network = (INetworkPredUpdater)Activator.CreateInstance(NetTypes[i], entry);
                var dt = Network.datatype.ToString();
                if (dt == entry.Datatype)
                {
                    Network.Propogate(entry, write);
                    break;
                }
            }
        }
        public static double[] PredictMulti(this Datatype dt, double[] input, WriteToCMDLine write)
        {
            Sample entry = new Sample(dt);
            entry.ValInput = input;

            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(y => !y.IsInterface).Where(x => type.IsAssignableFrom(x)).ToList();
            for (int i = 0; i < NetTypes.Count(); i++)
            {
                var Network = (INetworkPredUpdater)Activator.CreateInstance(NetTypes[i], entry);
                if (Network.datatype == dt)
                {
                    return Network.Predict(entry);
                }
            }
            return null;
        }
        public static void PropogateSingle(this Datatype dt, double[] input, double[] correct, WriteToCMDLine write)
        {
            Sample entry = new Sample(dt);
            entry.ValInput = input;
            entry.DesiredOutput = correct;

            var type = typeof(INetworkPredUpdater);
            Assembly a = type.Assembly;
            var NetTypes = a.GetTypes().Where(y => !y.IsInterface).Where(x => type.IsAssignableFrom(x)).ToList();
            for (int i = 0; i < NetTypes.Count(); i++)
            {
                var Network = (INetworkPredUpdater)Activator.CreateInstance(NetTypes[i], entry);
                if (Network.datatype == dt)
                {
                    Network.Propogate(entry, write);
                    break;
                }
            }
        }
    }
}
