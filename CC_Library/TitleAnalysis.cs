using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

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
    }
}
