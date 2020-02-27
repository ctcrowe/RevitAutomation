using System;

namespace CC_Library.Parameters
{
    public sealed class MasterformatParam : Param 
    {
        public string Category = "BIMData";
        public string name = "MASTERFORMAT";
        public int type = ParamType.String;
        public Guid ID = new Guid("98eefdd9-495d-4b4c-912b-aa7ce952b142");
        public Boolean Vis = true;
        public string Description = "A REFERENCE TO THE OBJECTS MASTERFORMAT DIVISION.";
        public Boolean UsrMod = false;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
}
