using System;

namespace CC_Library.Parameters
{
    public sealed class HardwareSet : Param
    {
        public string Category = "Doors";
        public string name = "HARDWARE SET";
        public int type = ParamType.String;
        public Guid ID = new Guid("c58d15e9-068f-486c-886c-7e6db68b97e1");
        public Boolean Vis = true;
        public string Description = "Hardware set of each type of door.";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
}
