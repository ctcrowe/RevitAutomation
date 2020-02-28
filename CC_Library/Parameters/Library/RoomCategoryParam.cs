using System;

namespace CC_Library.Parameters
{
    public sealed class RoomOccupancy : Param
    {
        public readonly string Category = "Rooms";
        public readonly string name = "ROOM CATEGORY";
        public readonly int type = ParamType.String;
        public readonly Guid ID = new Guid("819cfd72-96c9-4fff-b303-4765879f9747");
        public resadonly Boolean Vis = true;
        public readomly string Description = "A REFERENCE TO THE OCCUPANCY OF THE ROOM.";
        public readonly Boolean UsrMod = true;
        public readonly Boolean Inst = true;
        public readonly Boolean Fixed = true;
        public string Value { get; set; }
    }
}
