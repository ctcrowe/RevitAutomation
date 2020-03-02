using System;

namespace CC_Library.Parameters
{
    public sealed class IDParam : Param
    {
        private static readonly string _Category = "ID";
        private static readonly string _Name = "ID";
        private static readonly int _Type = ParamType.String;
        private static readonly Guid _ID = new Guid("dc2385d1-4c41-4a81-be07-834d54ed32a6");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "AN ID REFERENCE FOR DATA TRACKING";
        private static readonly Boolean _UsrMod = false;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;
        private static readonly int _Location = ParamLocation.Both;

        public IDParam() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
