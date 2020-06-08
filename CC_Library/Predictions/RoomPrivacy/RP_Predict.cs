using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions.RoomPrivacy
{
    public class RoomPrivacy
    {
        public static string Predict(string sample)
        {
            Dictionary<string, Data> MFData = Datatype.RoomPrivacy.Read();
            Dictionary<string, Data> DictData = Datatype.Dictionary.Read();

            var WordList = sample.SplitTitle();
            var DictionaryPoints = DictData.Where(x => WordList.Contains(x.Key));
            var WordPoint = DictionaryPoints.ResultantDatapoint();

            return MFData.FindClosest(WordPoint);
        }
    }
}
