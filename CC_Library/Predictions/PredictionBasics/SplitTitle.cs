using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Xml.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    public static class SplitTitle_Command
    {
        /*
        public static List<string> SplitTitle(this string s)
        {
            List<string> Array = new List<string>();
            List<string> data = new List<string>();
            char[] Characters = {'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g',
            'H', 'h', 'I', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o', 'P', 'p',
            'Q', 'q', 'R', 'r', 'S', 's', 'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y', 'Z', 'z'};
            char[] text = s.ToCharArray();
            string next = "";
            for (int i = 0; i < text.Count(); i++)
            {
                if (Characters.Contains(text[i]))
                    next += text[i];
                else
                {
                    if (next.Length >= 3)
                        Array.Add(next);
                    next = "";
                }
            }
            if (next.Length >= 3)
                Array.Add(next);
            foreach (string a in Array)
            {
                data.AddRange(a.SplitOnCaps());
            }
            return data;
        }*/
        public static List<string> GetWords(this string s)
        {
            List<string> output = new List<string>();
            /*
            var assembly = typeof(ReadWriteXML).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains("Dict.xml")))
            {
                string name = assembly.GetManifestResourceNames().Where(x => x.Contains("Dict.xml")).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                
                    XDocument doc = XDocument.Load(stream);*/
                    var word = s.ToUpper();
                    int length = word.Count();
                    int start = 0;
                    while (start < length - 1)
                    {
                        bool modified = false;
                        for (int j = length; j > start; j--)
                        {
                            string sub = word.Substring(start, j - start);
                            if(Enum.GetNames(typeof(Dict)).Contains(sub))
                            {
                                if (!output.Contains(sub))
                                {
                                    output.Add(sub);
                                }
                                start = j;
                                modified = true;
                                break;
                            }
                        }
                        if (!modified)
                            start++;
                    }
                    /*
                }
            }
            */
            return output;
        }
    }
}
