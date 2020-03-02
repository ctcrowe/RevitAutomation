using System;

namespace CC_Library.Parameters
{
    public sealed class HardwareSet : Param
    {
        private static readonly string _Category = "Doors";
        private static readonly string _Name = "HARDWARE SET";
        private static readonly int _Type = ParamType.String;
        private static readonly Guid _ID = new Guid("c58d15e9-068f-486c-886c-7e6db68b97e1");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "Hardware set of each type of door.";
        private static readonly Boolean _UsrMod = true;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;
        private static readonly int _Location = ParamLocation.Family;

        public HardwareSet() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
