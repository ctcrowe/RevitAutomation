using System;

namespace CC_Library.Parameters
{
    public sealed class RoomPrivacy : Param
    {
        public string Category = "Rooms";
        public string name = "ROOM PRIVACY";
        public int type = ParamType.Bool;
        public Guid ID = new Guid("4a521641-328a-4853-a919-030ace5474d0");
        public Boolean Vis = true;
        public string Description = "YES is private, NO is public";
        public Boolean UsrMod = true;
        public Boolean Inst = true;
        public Boolean Fixed = true;
    }
}
