using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
/*
namespace CC_Library.Predictions
{
    internal class GeneralPrediction
    {
        private const int RunSize = 8;

        public static void Propogate
            (string filepath,
            WriteToCMDLine write)
        {
            MasterformatNetwork mf = new MasterformatNetwork(write);
            DictionaryNetwork dict = new DictionaryNetwork(write);
            Alpha a = new Alpha(write);
            LocalContext MFlctxt = new LocalContext(Datatype.Masterformat, write);
            LocalContext DictlCtxt = new LocalContext(Datatype.Dictionary, write);
            GlobalContext MFgctxt = new GlobalContext(Datatype.Masterformat, write);
            GlobalContext DictgCtxt = new GlobalContext(Datatype.Dictionary, write);
            Random random = new Random();
            string[] Lines = File.ReadAllLines(filepath);

            mf.Network.Save();
            dict.Network.Save();
            a.Location.Save();
            MFlctxt.Save();
            DictlCtxt.Save();
            MFgctxt.Save();
            DictgCtxt.Save();
            write("Network Saved");

            int[] numbs = new int[Lines.Count()];
            for (int i = 0; i < numbs.Count(); i++)
                numbs[i] = i;

            int count = 1;
            var acc = new Accuracy(Lines);

            while (count < 3000000)
            {
                var numbers = numbs.OrderBy(x => random.Next()).ToList();

                for (int i = 0; i < Lines.Count() - (RunSize + 1); i += RunSize)
                {
                    write("Run Number : " + count);

                    Parallel.For(0, RunSize, j =>
                    {
                        switch(Lines[numbers[i + j]].Split(',').First())
                        {
                            case "Masterformat":
                            default:
                                MasterformatNetwork.SamplePropogate(Lines[numbers[i + j]], numbers[i + j], mf, a, MFlctxt, MFgctxt, acc, write);
                                break;
                            case "Dictionary":
                                DictionaryNetwork.SamplePropogate(Lines[numbers[i + j]], numbers[i + j], dict, a, DictlCtxt, DictgCtxt, acc, write);
                                break;
                        }
                    });
                    mf.Network.Update(RunSize, 0.01);
                    dict.Network.Update(RunSize, 0.01);
                    a.Location.Update(RunSize, 0.01);
                    MFlctxt.Network.Update(RunSize, 0.01);
                    DictlCtxt.Network.Update(RunSize, 0.01);
                    MFgctxt.Network.Update(RunSize, 0.01);
                    DictgCtxt.Network.Update(RunSize, 0.01);
                    acc.Show(write);
                    write("");

                    count++;
                }
                acc.Reset();
                mf.Network.Save();
                dict.Network.Save();
                a.Location.Save();
                MFlctxt.Save();
                DictlCtxt.Save();
                MFgctxt.Save();
                DictgCtxt.Save();
                write("Network Saved");
            }
        }
    }
}
*/