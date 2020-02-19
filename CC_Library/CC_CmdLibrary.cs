using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace CC_Library
{
    internal static class CMDLibrary
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }
        public static List<string> SplitOnCaps(this string s)
        {
            List<string> strings = new List<string>();
            int j = 0;
            char p = ' ';
            string outputstring = "";
            foreach(var c in s)
            {
                if (char.IsUpper(c) && !char.IsUpper(p) && p != ' ')
                {
                    strings.Add(outputstring);
                    outputstring = "" + c;
                }
                else
                {
                    outputstring += c;
                }
                if(j == s.Count() - 1)
                {
                    strings.Add(outputstring);
                }
                p = c;
                j++;
            }
            return strings;
        }
        public static List<string> SplitTitle(this string s)
        {
            List<string> data = new List<string>();
            char[] delimitters = { ',', '.', ' ', '_', '-' };
            List<string> Array = s.Split(delimitters).ToList();
            foreach(string a in Array)
            {
                data.AddRange(a.SplitOnCaps());
            }
            return data;
        }
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}