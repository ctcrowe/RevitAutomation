using System;

namespace CC_Library.Parameters
{
    public sealed class FamilyName : Param
    {
        private static readonly string _Category = "ID";
        private static readonly string _Name = "FNAME";
        private static readonly int _Type = ParamType.String;
        private static readonly Guid _ID = new Guid("193b3ca2-da43-468f-adb2-3d8d4d300749");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "A FAMILY NAME REFERENCE FOR DATA TRACKING";
        private static readonly  Boolean _UsrMod = false;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;
        private static readonly int _Location = ParamLocation.Family;

        public FamilyName() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
