using System;

namespace CC_Library.Parameters
{
    public static class Params
    {
        public static readonly Param ViewID = new Param
            ("ViewID",
            new Guid("bc77260bddda4eaa835fe21a4e95dd50"),
            Subcategory.View,
            ParamType.Text,
            true,
            false);
        public static readonly Param Masterformat = new Param
            ("MF Division",
            new Guid("aedbf7d2d73c44048cf7ed26f3231d25"),
            Subcategory.Generic,
            ParamType.Int,
            false,
            true);
        public static readonly Param Masterformat_Material = new Param
            ("MF Division Material",
            new Guid("c0409866752d448b9ae10c03942ba603"),
            Subcategory.Materials,
            ParamType.Int,
            true,
            false);
    }
}