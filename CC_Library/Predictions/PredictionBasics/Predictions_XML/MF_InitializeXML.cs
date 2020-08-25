using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CC_Library.Datatypes;

/*
namespace CC_Library.Predictions
{
    internal static class MF_InitializeXML
    {
        public static void InitializeMasterformat(this List<XDocument> XDocs, Dictionary<string, string[]> entries)
        {
            foreach(string[] entry in entries.Values)
            {
                foreach(string e in entry)
                {
                    XDocument doc = Datatype.Masterformat.GetElement(e);
                    if (!XDocs.Any(x => x.Root.Name.ToString() == doc.Root.Name.ToString()))
                        XDocs.Add(doc);
                }
            }
        }
        public static void InitializeDictionary(this List<XDocument> XDocs, Dictionary<string, string[]> entries)
        {
            foreach (string entry in entries.Keys)
            {
                foreach (string e in entry.SplitTitle())
                {
                    XDocument doc = Datatype.Dictionary.GetElement(e);
                    if (!XDocs.Any(x => x.Root.Name.ToString() == doc.Root.Name.ToString()))
                        XDocs.Add(doc);
                }
            }
        }
    }
}
*/