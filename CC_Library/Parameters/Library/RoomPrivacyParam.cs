using System;

namespace CC_Library.Parameters
{
    public sealed class RoomPrivacy : Param
    {
        private static readonly string _Category = "Rooms";
        private static readonly string _Name = "ROOM PRIVACY";
        private static readonly int _Type = ParamType.Bool;
        private static readonly Guid _ID = new Guid("4a521641-328a-4853-a919-030ace5474d0");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "YES is private, NO is public";
        private static readonly Boolean _UsrMod = true;
        private static readonly Boolean _Inst = true;
        private static readonly Boolean _Fixed = true;
        private static readonly int _Location = ParamLocation.Space;

        public RoomPrivacy() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
