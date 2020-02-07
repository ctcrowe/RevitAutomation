using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;

namespace CC_Library
{
    internal class PredictionElement
    {
        public string Word { get; }
        internal List<PredictionOption> Options { get; set; }

        public PredictionElement(string w)
        {
            this.Word = w;
            this.Options = new List<PredictionOption>();
        }
        public PredictionElement(XElement ele)
        {
            this.Word = ele.Attribute("WORD").Value;
            this.Options = new List<PredictionOption>();
            foreach (XElement e in ele.Elements("OPTION"))
            {
                if (!Options.Any(x => x.Name == e.Attribute("NAME").Value))
                    Options.Add(new PredictionOption(e));
            }
        }
        public XElement CreateElement()
        {
            XElement ele = new XElement("ELEMENT");
            ele.Add(new XAttribute("WORD", Word));
            foreach (PredictionOption p in Options)
            {
                ele.Add(p.CreateOption());
            }
            return ele;
        }
        public void AddOption(string s)
        {
            if (!Options.Any(x => x.Name == s))
                Options.Add(new PredictionOption(s));
        }
    }
}
