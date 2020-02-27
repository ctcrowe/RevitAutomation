using System;

namespace CC_Library.Parameters
{
    public sealed class FireRating : Param
    {
        public string Category = "FireProtection";
        public string name = "FIRE RATING";
        public int type = ParamType.String;
        public Guid ID = new Guid("4333b0f4-1652-4d7a-8060-2044707e0815");
        public Boolean Vis = true;
        public string Description = "Fire Ratings foor Doors, Windows, and Walls.";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
}
