using System;

namespace CC_Library.Parameters
{
    public sealed class RoomOccupancy : Param
    {
        public string Category = "Rooms";
        public string name = "ROOM CATEGORY";
        public int type = ParamType.String;
        public Guid ID = new Guid("819cfd72-96c9-4fff-b303-4765879f9747");
        public Boolean Vis = true;
        public string Description = "A REFERENCE TO THE OCCUPANCY OF THE ROOM.";
        public Boolean UsrMod = true;
        public Boolean Inst = true;
        public Boolean Fixed = true;
    }
}
