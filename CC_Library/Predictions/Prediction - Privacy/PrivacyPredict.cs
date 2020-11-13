using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class PrivacyPredict
    {
        public static string RPPredict(this string s)
        {
            List<string> words = s.GetWords();
            Dictionary<string, Element> DictPoints = new Dictionary<string, Element>();
            var Dataset = Datatype.RoomPrivacy.GetElements().ToDictionary(x => x.Label, y => y);

            foreach (var word in words)
            {
                DictPoints.Add(word, Datatype.Dictionary.GetElement(word));
            }
            var Combined = DictPoints.Combine();
            return Dataset.FindClosest(Combined);
        }
    }
}
