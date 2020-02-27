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
    public sealed class Depth : Param
    {
        public string Category = "Dimensions";
        public string name = "oD";
        public int type = ParamType.Length;
        public Guid ID = new Guid("93acc448-a229-4d27-956f-c39ffd40c3c3");
        public Boolean Vis = true;
        public string Description = "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING FRONT AND BACK EXTREMES";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
    public sealed class Height : Param
    {
        public string Category = "Dimensions";
        public string name = "oH";
        public int type = ParamType.Length;
        public Guid ID = new Guid("f306ae6d-f153-47e1-b7e1-ac451eada6f2");
        public Boolean Vis = true;
        public string Description = "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING TOP AND BOTTOM EXTREMES";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
}
