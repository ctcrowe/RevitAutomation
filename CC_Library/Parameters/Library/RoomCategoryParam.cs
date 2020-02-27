using System;

namespace CC_Library.Parameters
{
    public sealed class RoomCategory : Param
    {
        public string Category = "Rooms";
        public string name = "ROOM CATEGORY";
        public int type = ParamType.String;
        public Guid ID = new Guid("3231dffc-7322-43b6-9ede-d4e5fbb80399");
        public Boolean Vis = true;
        public string Description = "A REFERENCE TO THE USE OF THE ROOM.";
        public Boolean UsrMod = false;
        public Boolean Inst = true;
        public Boolean Fixed = true;
    }
}
