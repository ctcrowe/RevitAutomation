using System;

namespace CC_Library.Parameters
{
    class FamilyName
    {
        public string Category = "ID";
        public string name = "FNAME";
        public int type = ParamType.String;
        public Guid ID = new Guid("193b3ca2-da43-468f-adb2-3d8d4d300749");
        public Boolean Vis = true;
        public string Description = "A FAMILY NAME REFERENCE FOR DATA TRACKING";
        public Boolean UsrMod = false;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
}
