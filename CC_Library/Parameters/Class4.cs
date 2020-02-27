using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace CC_Library.Parameters
{

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
