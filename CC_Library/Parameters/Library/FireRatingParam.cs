using System;

namespace CC_Library.Parameters
{
    public sealed class FireRating : Param
    {
        public readonly string Category = "FireProtection";
        public readonly string name = "FIRE RATING";
        public readonly int type = ParamType.String;
        public readonly Guid ID = new Guid("4333b0f4-1652-4d7a-8060-2044707e0815");
        public readonly Boolean Vis = true;
        public readonly string Description = "Fire Ratings foor Doors, Windows, and Walls.";
        public readonly Boolean UsrMod = true;
        public readonly Boolean Inst = false;
        public readonly Boolean Fixed = true;
        public string Value { get; set; }
    }
}
