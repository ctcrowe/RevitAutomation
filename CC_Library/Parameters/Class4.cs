using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace CC_Library.Parameters
{
    public abstract class Param
    {
        public string ParamGroup;
        public string name;
        public int type;
        public Guid ID;
        public Boolean Vis;
        public string Description;
        public Boolean UsrMod;
        public Boolean Inst;
        public Boolean Fixed;
    }
    public static class ParamType
    {
        public const int Bool = 0;
        public const int Int = 1;
        public const int Double = 2;
        public const int Length = 3;
        public const int String = 4;
        public const int Material = 5;
        public const int Area = 6;
    }
    public class DataCouplet
    {
        Param P { get; set;}
        string V { get; set;}
        
        public DataCouplet(Param p, string v)
        {
            this.P = p;
            this.V = v;
        }
        public DataCouplet(Param p)
        {
            this.P = p;
            this.V = string.Empty;
        }
    }
    public class CC_Element
    {
        string ID { get; }
        DataCouplet[] Dataset { get; set; }
        public CC_Element(DataCouplet[] data)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Dataset = data;
        }
    }
}
