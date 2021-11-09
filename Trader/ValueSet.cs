﻿using System;
using System.Collections.Generic;
using System.Text;
using CC_Library.Predictions;

namespace Trader
{
    [Serializable]
    public class ValueSet
    {
        public GUID guid { get; set; }
        public List<StonkValues> bars { get; set; }
        public StonkValues TargetBar { get; set; }
        public int Output { get; set; }

        public ValueSet()
        {
            this.bars = new List<StonkValues>();
            this.Output = 0;
        }
        public void SaveTxt()
        {
            List<string> Lines = new List<string>();
            Lines.Add("Input GUID " + guid.ToString());
        }
        public ValueSet LoadTxt(string fn)
        {
            return new ValueSet();
        }
        public void SaveBin()
        {

        }
    }
}
