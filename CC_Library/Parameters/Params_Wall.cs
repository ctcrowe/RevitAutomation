using System;

namespace CC_Library.Parameters
{
    public static class WallParams
    {
        private static Subcategory sub = Subcategory.Wall;
        public static readonly Param FireRating = new Param
            ("FireRating",
            new Guid("a3abf061b101424683a9d6ba022db960"),
            sub,
            ParamType.Text,
            false);
        public static readonly Param FilterFlags = new Param
            ("FilterFlags",
            new Guid("540c2d7d67374efa8797135958ee446d"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param Thickness = new Param
            ("Thickness",
            new Guid("bd83390498b94237bd5273e758084c6a"),
            sub,
            ParamType.Length,
            false);
        public static readonly Param StudSize = new Param
            ("StudSize",
            new Guid("dba7d515a4b34f55a374f5744367206f"),
            sub,
            ParamType.Length,
            false);
        public static readonly Param ULListing = new Param
            ("ULListing",
            new Guid("bea192bd04e54374815266e7819fb26b"),
            sub,
            ParamType.Text,
            false);
        public static readonly Param InScope = new Param
            ("InScope",
            new Guid("a533a5849a7c48b6ac2ae37b9e2718ba"),
            sub,
            ParamType.Bool,
            true);
    }
}