using System;

namespace CC_Library.Parameters
{
    public sealed class MasterformatParam : Param 
    {
        public readonly string Category = "BIMData";
        public readonly string name = "MASTERFORMAT";
        public readonly int type = ParamType.String;
        public readonly Guid ID = new Guid("98eefdd9-495d-4b4c-912b-aa7ce952b142");
        public readonly Boolean Vis = true;
        public readonly string Description = "A REFERENCE TO THE OBJECTS MASTERFORMAT DIVISION.";
        public readonly Boolean UsrMod = false;
        public readonly Boolean Inst = false;
        public readonly Boolean Fixed = true;
        public string Value {get; set;}
    }
}
