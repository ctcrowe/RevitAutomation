using System;

namespace CC_Library.Parameters
{
    public sealed class HeightParam : Param
    {
        private static readonly string _Category = "Dimensions";
        private static readonly string _Name = "oH";
        private static readonly int _Type = ParamType.Length;
        private static readonly Guid _ID = new Guid("f306ae6d-f153-47e1-b7e1-ac451eada6f2");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING TOP AND BOTTOM EXTREMES";
        private static readonly Boolean _UsrMod = true;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;

        public HeightParam() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed) { }
    }
}
