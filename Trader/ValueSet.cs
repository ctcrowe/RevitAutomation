using System;
using System.Collections.Generic;
using System.Text;
using CC_Library.Predictions;

namespace Trader
{
    [Serializable]
    public class ValueSet
    {
        public Guid guid { get; set; }
        public List<StonkValues> Values { get; set; }
        public StonkValues Target { get; set; }
        public int Output { get; set; }

        public ValueSet()
        {
            this.guid = Guid.NewGuid();
            this.Values = new List<StonkValues>();
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
