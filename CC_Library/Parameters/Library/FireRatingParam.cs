using System;

namespace CC_Library.Parameters
{
    public sealed class FireRating : Param
    {
        private static readonly string _Category = "FireProtection";
        private static readonly string _Name = "FIRE RATING";
        private static readonly int _Type = ParamType.String;
        private static readonly Guid _ID = new Guid("4333b0f4-1652-4d7a-8060-2044707e0815");
        private static readonly Boolean _Vis = true;
        private static readonly string _Description = "Fire Ratings foor Doors, Windows, and Walls.";
        private static readonly Boolean _UsrMod = true;
        private static readonly Boolean _Inst = false;
        private static readonly Boolean _Fixed = true;
        private static readonly int _Location = ParamLocation.Wall;

        public FireRating() : base(_Category, _Name, _Type, _ID, _Vis, _Description, _UsrMod, _Inst, _Fixed, _Location) { }
    }
}
