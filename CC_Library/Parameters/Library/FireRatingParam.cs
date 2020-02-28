using System;

namespace CC_Library.Parameters
{
    public sealed class FireRating : Param
    {
        public static readonly string Category = "FireProtection";
        public static readonly string name = "FIRE RATING";
        public static readonly int type = ParamType.String;
        public static readonly Guid ID = new Guid("4333b0f4-1652-4d7a-8060-2044707e0815");
        public static readonly Boolean Vis = true;
        public static readonly string Description = "Fire Ratings foor Doors, Windows, and Walls.";
        public static readonly Boolean UsrMod = true;
        public static readonly Boolean Inst = false;
        public static readonly Boolean Fixed = true;
    }
}
