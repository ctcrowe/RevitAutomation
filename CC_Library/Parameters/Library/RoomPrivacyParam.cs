using System;

namespace CC_Library.Parameters
{
    public sealed class RoomPrivacy : Param
    {
        public readonly string Category = "Rooms";
        public readonly string name = "ROOM PRIVACY";
        public readonly int type = ParamType.Bool;
        public readonly Guid ID = new Guid("4a521641-328a-4853-a919-030ace5474d0");
        public readonly Boolean Vis = true;
        public readonly string Description = "YES is private, NO is public";
        public readonly Boolean UsrMod = true;
        public readonly Boolean Inst = true;
        public readonly Boolean Fixed = true;
        public string Value { get; set; }
    }
}
