using System;

namespace CC_Library.Parameters
{
    public static class MaterialParams
    {
        private static Subcategory sub = Subcategory.Materials;
        public static readonly Param CC_ID = new Param
            ("CC_ID",
            new Guid("e8bfeab8a49549238cbaa7a402f59d57"),
            sub,
            ParamType.Text,
            true,
            false);
        public static readonly Param Category = new Param
            ("Category",
            new Guid("f1806391703247118e6978910e72a4d5"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param Manufacturer = new Param
            ("Manufacturer",
            new Guid("b7200c06c5ef4507874b1a49b7ef0043"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param Model = new Param
            ("Model",
            new Guid("b206cea7ffe4400ca2cd314c6dea3874"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param ColorFinish = new Param
            ("ColorFinish",
            new Guid("e3ed247d44b944c1834ebe864f082581"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param PatternStyle = new Param
            ("PatternStyle",
            new Guid("a4ef2c1e6acb44539af197b21e79dba1"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param MatDescription = new Param
            ("MatDescription",
            new Guid("f297a6cec3eb4922a421c466e915a53a"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param FinishMaterial = new Param
            ("FinishMaterial",
            new Guid("b8b5c2b1c93c458c9ab5ed7237efdd85"),
            sub,
            ParamType.Bool,
            true);
        public static readonly Param FilterFlags = new Param
            ("FilterFlags",
            new Guid("540c2d7d67374efa8797135958ee446d"),
            sub,
            ParamType.Text,
            true);
    }
}
