using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    internal static class XmlElementToChange
    {
        public static XDocument ElementToChange
            (this KeyValuePair<string, bool> Change,
            List<XDocument> Dataset,
            List<XDocument> Dictset)
        {
            if (Change.Value)
                return Dataset.Where(x => x.Root.Attribute("Label").Value == Change.Key).First();
            else
                return Dictset.Where(x => x.Root.Attribute("Label").Value == Change.Key).First();
        }
    }
}
