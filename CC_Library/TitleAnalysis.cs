using System.Linq;
using System.Collections.Generic;

namespace CC_Library
{
    public class TitleAnalysis
    {
        public string Title { get; }
        public int Section { get; }

        public TitleAnalysis(string s, int i)
        {
            this.Title = s;
            this.Section = i;
        }
        
        public static List<TitleAnalysis> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<TitleAnalysis>();

            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        if (!data.Any(x => x.Title == ele))
                        {
                            if(doc.Root.Attribute("Section") != null)
                            {
                                data.Add(new TitleAnalysis(ele, doc.Root.Attribute("Section").Value));
                            }
                            else
                            {
                                data.Add(new TitleAnalysis(ele, 0));
                            }
                        }
                    }
                }
            }
            return data;
        }
    }
}
