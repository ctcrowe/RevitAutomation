using System;

namespace CC_Library.Parameters
{
    public static class DoorParams
    {
        private static Subcategory sub = Subcategory.Doors;
        public static readonly Param CC_ID = new Param
            ("CC_ID",
            new Guid("e8bfeab8a49549238cbaa7a402f59d57"),
            sub,
            ParamType.Text,
            true,
            false);
        public static readonly Param OverallHeight = new Param
            ("OverallHeight",
            new Guid("cfc88653c63b47efb165380d7e63600c"),
            sub,
            ParamType.Length,
            false);
        public static readonly Param OverallWidth = new Param
            ("OverallWidth",
            new Guid("d0249e62f16141868704e89c096cbbc9"),
            sub,
            ParamType.Length,
            false);
        public static readonly Param PanelWidth = new Param
            ("DoorPanelWidth",
            new Guid("05fd515c1ae2405d98859c383ce34840"),
            sub,
            ParamType.Length,
            false);
        public static readonly Param PanelHeight = new Param
            ("DoorPanelHeight",
            new Guid("038a624ce2924c659e46bccb070231b3"),
            sub,
            ParamType.Length,
            false);
        public static readonly Param PanelThickness = new Param
            ("DoorPanelThickness",
            new Guid("ed9dcf40ab384c94a1c97337ff56e18f"),
            sub,
            ParamType.Length,
            false);
        public static readonly Param PanelFinish = new Param
            ("PanelFinish",
            new Guid("b1faed092fed4be4ad760bde8d80320a"),
            sub,
            ParamType.Material,
            true);
        public static readonly Param FrameFinish = new Param
            ("FrameFinish",
            new Guid("ec32cfe55b2e41a9aa76c390643acd2b"),
            sub,
            ParamType.Material,
            true);
        public static readonly Param PanelMaterial = new Param
            ("PanelMaterial",
            new Guid("ceaa6e3810dd4d23a061e6f11bd799d2"),
            sub,
            ParamType.Material,
            false);
        public static readonly Param FrameMaterial = new Param
            ("FrameMaterial",
            new Guid("e7bece7b3ce24dd7a26b069f3e60e9c6"),
            sub,
            ParamType.Material,
            false);
        public static readonly Param WallThickness = new Param
            ("WallThickness",
            new Guid("fc75b64a7e4b40a380f34835f6ce09a4"),
            sub,
            ParamType.Length,
            true);
        public static readonly Param HardwareGroup = new Param
            ("HardwareGroup",
            new Guid("c17ded9a28a24ac9a9b8b079877e9016"),
            sub,
            ParamType.Text,
            false);
        public static readonly Param HeadDetail = new Param
            ("HeadDetail",
            new Guid("c9ba846fd1f7459791daab1f109a88d6"),
            sub,
            ParamType.Text,
            false);
        public static readonly Param JambDetail = new Param
            ("JambDetail",
            new Guid("bfe5dc8dfdad4e508f764c0352cacbd5"),
            sub,
            ParamType.Text,
            false);
        public static readonly Param SillDetail = new Param
            ("SillDetail",
            new Guid("e82782e8c18140268b8c83ba4af92021"),
            sub,
            ParamType.Text,
            false);
        public static readonly Param FireRating = new Param
            ("FireRating",
            new Guid("a3abf061b101424683a9d6ba022db960"),
            sub,
            ParamType.Text,
            false);
        public static readonly Param Glazing = new Param
            ("GlazingType",
            new Guid("1802e784b4b2415bb205efe7f5031e52"),
            sub,
            ParamType.Material,
            false);
        public static readonly Param FilterFlags = new Param
            ("FilterFlags",
            new Guid("540c2d7d67374efa8797135958ee446d"),
            sub,
            ParamType.Text,
            true);
    }
}