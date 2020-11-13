using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class LoadFactorPredict
    {
        private static void writeout(string s) { }
        public static string OLFPredict(this string s)
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

                switch (result.ToList().IndexOf(result.Max()))
                {
                    case 0:
                        return "5";
                    case 1:
                        return "7";
                    case 2:
                        return "11";
                    case 3:
                        return "15";
                    case 4:
                        return "20";
                    case 5:
                        return "30";
                    case 6:
                        return "35";
                    case 7:
                        return "40";
                    case 8:
                        return "50";
                    case 9:
                        return "60";
                    default:
                    case 10:
                        return "100";
                    case 11:
                        return "120";
                    case 12:
                        return "200";
                    case 13:
                        return "240";
                    case 14:
                        return "300";
                    case 15:
                        return "500";
                }
            }
            return "";
        }
    }
}