using System;

namespace CC_Library.Parameters
{
    public sealed class MasterformatParam : Param 
    {
        private static readonly string _Category = "BIMData";
        private static readonly string _Name = "MASTERFORMAT";
        private static readonly int _Type = ParamType.String;
        private static readonly Guid _ID = new Guid("98eefdd9-495d-4b4c-912b-aa7ce952b142");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "A REFERENCE TO THE OBJECTS MASTERFORMAT DIVISION.";
        private static readonly Boolean _UsrMod = false;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;
        private static readonly ParamLocation _Location = ParamLocation.Family;

        public MasterformatParam() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
