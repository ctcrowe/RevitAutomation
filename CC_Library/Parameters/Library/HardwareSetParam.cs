using System;

namespace CC_Library.Parameters
{
    public sealed class HardwareSet : Param
    {
        public readonly string Category = "Doors";
        public readonly string name = "HARDWARE SET";
        public readonly int type = ParamType.String;
        public readonly Guid ID = new Guid("c58d15e9-068f-486c-886c-7e6db68b97e1");
        public readonly Boolean Vis = true;
        public readonly string Description = "Hardware set of each type of door.";
        public readonly Boolean UsrMod = true;
        public readonly Boolean Inst = false;
        public readonly Boolean Fixed = true;
        public string Value {get; set;}
    }
}
