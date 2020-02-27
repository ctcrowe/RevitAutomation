using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Parameters
{
    public sealed class Width : Param
    {
        public string Category = "Dimensions";
        public string name = "oW";
        public int type = ParamType.Length;
        public Guid ID = new Guid("7c53c6ed-278f-4036-b5c7-eac8437ab28a");
        public Boolean Vis = true;
        public string Description = "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING LEFT AND RIGHT EXTREMES";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
}
