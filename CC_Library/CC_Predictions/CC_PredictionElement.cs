﻿using System.Linq;
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
        public List<PredictionOption> Options { get; set; }
        public int Count { get; set; }

        public PredictionElement(string w)
        {
            this.Word = w;
            this.Options = new List<PredictionOption>();
            this.Count = 0;
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
        private XElement CreateElement()
        {
            XElement ele = new XElement("ELEMENT");
            ele.Add(new XAttribute("WORD", Word));
            foreach (PredictionOption p in Options)
            {
                ele.Add(p.CreateOption());
            }
            return ele;
        }
        public void AdjustElement(XElement root)
        {
            if(root.Elements("ELEMENT").Any(x => x.Attribute("WORD").Value == Word))
            {
                XElement e = root.Elements("ELEMENT").Where(x => x.Attribute("WORD").Value == Word);
            }
            else
            {
                root.Add(CreateElement());
            }
        }
        public void AddOption(string s)
        {
            if (!Options.Any(x => x.Name == s))
            {
                Options.Add(new PredictionOption(s));
            }
            Options.Where(x => x.Name == s).First().AddPositive();
            Count++;
        }
        public void SubtractOption(string s)
        {
            if(!Options.Any(x => x.Name == s))
            {
                Option.Add(new PredictionOption(s));
            }
            Options.Where(x => x.Name == s).AddNegative();
        }
    }
}