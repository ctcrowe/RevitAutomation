using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using CC_Librariy;
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
            string folder = "ValueSets".GetMyDocs();
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string filename = folder + "\\" + guid.ToString();
            
            List<string> Lines = new List<string>();
            Lines.Add("GUID : " + guid.ToString());
            foreach(var val in Values)
            {
                Lines.Add(GetValueText(val));
            }
            File.WriteAllLines(filename, Lines);
        }
        public string GetValueText(StonkValues value)
        {
            string l = "";
            l+= value.Symbol + ",";
            l+= value.Time.ToString() + ",";
            l+= value.AskPrice.ToString() + ",";
            l+= value.BidPrice.ToString();
        }
        public StonkValues GetValue(string s)
        {
            var l = s.Split(',');
            var symb = l[0];
            var time = DateTime.Parse(l[2]);
            var ask = double.Parse(l[3]);
            var bid = double.Parse(l[4]);
            StonkValues val = new StonkValue(symb, time, ask, bid);
        }
    }
}
