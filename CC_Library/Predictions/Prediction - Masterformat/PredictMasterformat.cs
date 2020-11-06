using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class MasterformatPredict
    {
        private static void writeout(string s) { }
        public static string MFPredict(this string s)
        {
            var Network = LoadNeuralNetwork.LoadNetwork(Datatype.Masterformat, new List<string>(), new WriteToCMDLine(writeout));
            var WordList = s.GetWords();
            if (WordList.Any())
            {
                var result = WordList.GetInput();
                var Zees = new double[1];
                for (int j = 0; j < Network.Layers.Count(); j++)
                {
                    Zees = Network.Layers[j].ZScore(result);
                    result = Network.Layers[j].Output(Zees);
                }
                
                return result.ToList().IndexOf(result.Max()).ToString();
            }
            return "";
        }
    }
}