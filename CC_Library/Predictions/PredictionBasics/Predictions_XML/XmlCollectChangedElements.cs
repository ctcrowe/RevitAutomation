using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    internal static class XmlCollectChangedElements
    {
        public static Dictionary<string, bool> CollectChangedElements
            (this int changecount,
            List<XDocument> DataSet,
            List<XDocument> DictSet)
        {
            IEnumerable<XDocument> SortedData = DictSet.Concat(DataSet);
            SortedData = SortedData.OrderBy(x => double.Parse(x.Root.Attribute("Accuracy").Value)).
                ThenBy(x => int.Parse(x.Root.Attribute("Total").Value));
            Dictionary<string, bool> changed = new Dictionary<string, bool>();

            for (int i = 0; i < changecount; i++)
            {
                if (DictSet.Any(x => x.Root.Attribute("Label").Value == SortedData.ElementAt(i).Root.Attribute("Label").Value))
                    changed.Add(SortedData.ElementAt(i).Root.Attribute("Label").Value, false);
                else
                    changed.Add(SortedData.ElementAt(i).Root.Attribute("Label").Value, true);
            }

            return changed;
        }
    }
}
