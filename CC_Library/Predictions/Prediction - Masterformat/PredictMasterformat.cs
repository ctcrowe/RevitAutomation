using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class MasterformatPredict
    {
        public static string MFPredict(this string s)
        {
            List<string> words = s.SplitTitle();
            Dictionary<string, Element> DictPoints = new Dictionary<string, Element>();
            var Dataset = Datatype.Masterformat.GetElements().ToDictionary(x => x.Label, y => y);

            foreach (var word in words)
            {
                DictPoints.Add(word, Datatype.Dictionary.GetElement(word));
            }
            var Combined = DictPoints.Combine();
            return Dataset.FindClosest(Combined);
        }
    }
}
