using System;

namespace CC_Library.Parameters
{
    public sealed class WidthParam : Param
    {
        private static readonly string _Category = "Dimensions";
        private static readonly string _Name = "oW";
        private static readonly int _Type = ParamType.Length;
        private static readonly Guid _ID = new Guid("7c53c6ed-278f-4036-b5c7-eac8437ab28a");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING LEFT AND RIGHT EXTREMES";
        private static readonly Boolean _UsrMod = true;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;
        private static readonly ParamLocation _Location = ParamLocation.Family;

        public WidthParam() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
