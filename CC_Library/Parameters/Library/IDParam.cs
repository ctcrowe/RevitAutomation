using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Parameters
{
    public class IDParam
    {
        public string Category = "ID";
        public string name = "ID";
        public int type = ParamType.String;
        public Guid ID = new Guid("dc2385d1-4c41-4a81-be07-834d54ed32a6");
        public Boolean Vis = true;
        public string Description = "AN ID REFERENCE FOR DATA TRACKING";
        public Boolean UsrMod = false;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
}
