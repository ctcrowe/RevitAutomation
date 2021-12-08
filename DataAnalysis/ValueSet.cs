using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using CC_Library;
using CC_Library.Predictions;

namespace DataAnalysis
{
    [Serializable]
    public class ValueSet
    {
        public Guid guid { get; set; }
        public List<StonkValues> Values { get; set; }
        public StonkValues FinValue { get; set; }
        public StonkValues CompValue { get; set; }
        public bool Increase { get; set; }

        public ValueSet()
        {
            this.guid = Guid.NewGuid();
            this.Values = new List<StonkValues>();
        }
        public void SaveTxt()
        {
            string folder = "ValueSets".GetMyDocs();
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string filename = folder + "\\" + guid.ToString() + ".txt";

            List<string> Lines = new List<string>();
            Lines.Add("GUID : " + guid.ToString());
            Lines.Add("Increase : " + Increase);
            Lines.Add("Fin Value : " + GetValueText(FinValue));
            Lines.Add("Comp Value : " + GetValueText(CompValue));
            foreach (var val in Values)
            {
                Lines.Add(GetValueText(val));
            }
            File.WriteAllLines(filename, Lines);
            this.Save();
        }
        public static ValueSet Load(string filename)
        {
        }
        private string GetValueText(StonkValues value)
        {
            string l = "";
            l+= value.Symbol + ",";
            l+= value.Time.ToString() + ",";
            l+= value.AskPrice.ToString() + ",";
            l+= value.BidPrice.ToString();
            return l;
        }
        public StonkValues GetValue(string s)
        {
            var l = s.Split(',');
            var symb = l[0];
            var time = DateTime.Parse(l[2]);
            var ask = double.Parse(l[3]);
            var bid = double.Parse(l[4]);
            StonkValues val = new StonkValues(symb, time, ask, bid);
            return val;
        }
    }
    public static class SaveValueSet
    {
        public static void Save(this ValueSet set)
        {
            string Folder = "ValueSets".GetMyDocs();
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            string FileName = Folder + "\\" + set.guid.ToString() + ".bin";
            WriteToBinaryFile(FileName, set, true);
        }
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Create(filePath))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
    }
}
