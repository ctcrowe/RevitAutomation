using System;

namespace CC_Library.Parameters
{
    public sealed class RoomOccupancy : Param
    {
        private static readonly string _Category = "Rooms";
        private static readonly string _Name = "ROOM CATEGORY";
        private static readonly int _Type = ParamType.String;
        private static readonly Guid _ID = new Guid("819cfd72-96c9-4fff-b303-4765879f9747");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "A REFERENCE TO THE OCCUPANCY OF THE ROOM.";
        private static readonly Boolean _UsrMod = true;
        private static readonly Boolean _Inst = true;
        private static readonly Boolean _Fixed = true;
        private static readonly ParamLocation _Location = ParamLocation.Space;

        public RoomOccupancy() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
