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

        public ValueSet()
        {
            this.guid = Guid.NewGuid();
            this.Values = new List<StonkValues>();
            this.Output = 0;
        }
        public void SaveTxt()
        {
            List<string> Lines = new List<string>();
            Lines.Add("GUID : " + guid.ToString());
            foreach(var val in Values)
            {
                Lines.Add(GetValueText(val));
            }
        }
        public string GetValueText(StonkValues value)
        {
            string l = "";
            l+= value.Symbol + ",";
            l+= value.id + ",";
            l+= value.AskPrice.ToString() + ",";
            l+= value.BidPrice.ToString();
        }
    }
}
