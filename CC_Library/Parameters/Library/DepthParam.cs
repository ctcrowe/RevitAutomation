using System;

namespace CC_Library.Parameters
{
    public sealed class DepthParam : Param
    {
        private static readonly string _Category = "Dimensions";
        private static readonly string _Name = "oD";
        private static readonly int _Type = ParamType.Length;
        private static readonly Guid _ID = new Guid("93acc448-a229-4d27-956f-c39ffd40c3c3");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING FRONT AND BACK EXTREMES";
        private static readonly Boolean _UsrMod = true;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;

        public DepthParam() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed) { }
    }
}