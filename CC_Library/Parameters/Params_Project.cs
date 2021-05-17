using System;

namespace CC_Library.Parameters
{
    public static class ProjectParams
    {
        private static Subcategory sub = Subcategory.Project;
        public static readonly Param CC_ID = new Param
            ("CC_ID",
            new Guid("e8bfeab8a49549238cbaa7a402f59d57"),
            sub,
            ParamType.Text,
            true,
            false);
        public static readonly Param Families = new Param
            ("Families",
            new Guid("cbfb6999dd6a44088ea9854021f66520"),
            sub,
            ParamType.Int,
            true,
            false);
        public static readonly Param FamilyInstances = new Param
            ("FamilyInstances",
            new Guid("c66e2d0e4a984235957211fbbf3e034d"),
            sub,
            ParamType.Int,
            true,
            false);
        public static readonly Param Materials = new Param
            ("Materials",
            new Guid("df6d39782b5b4899a2bfb0224b407e33"),
            sub,
            ParamType.Int,
            true,
            false);
        public static readonly Param RefPlanes = new Param
            ("RefPlanes",
            new Guid("476e9ac63f76499789d42293c0e3fbf3"),
            sub,
            ParamType.Int,
            true,
            false);
        public static readonly Param Sheets = new Param
            ("Sheets",
            new Guid("7f7c14a960ec495c88472b2109a64934"),
            sub,
            ParamType.Int,
            true,
            false);
        public static readonly Param Views = new Param
            ("Views",
            new Guid("7f0b2913062540168d3352683a6ee6f5"),
            sub,
            ParamType.Int,
            true,
            false);
        public static readonly Param ViwesOnSheets = new Param
            ("ViewsOnSheets",
            new Guid("cb9b713ce6fa4a9f8cba1d85153dc0a6"),
            sub,
            ParamType.Int,
            true,
            false);
        public static readonly Param ModelLines = new Param
            ("ModelLines",
            new Guid("b988a84df7ad4833b0a53f3acd0b4634"),
            sub,
            ParamType.Int,
            true,
            false);
    }
}