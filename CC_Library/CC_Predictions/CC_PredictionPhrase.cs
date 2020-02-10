
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;

namespace CC_Library
{
    internal class PredictionPhrase
    {
        public string Phrase { get; }
        public string Prediction { get; set; }
                
        public PredictionPhrase(string i, string o)
        {
            this.Phrase = i;
            this.Prediction = o;
        }
    }
}
