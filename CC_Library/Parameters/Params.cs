using System;

namespace CC_Library.Parameters
{
    public static class Params
    {
        public static readonly Param Masterformat = new Param
            ("MF Division",
            new Guid("aedbf7d2d73c44048cf7ed26f3231d25"),
            Subcategory.Generic,
            ParamType.Int,
            false,
            true);
        public static readonly Param Finish = new Param
            ("Finish Material",
            new Guid("4848e7f4ee234755b9892deb5eef17b6"),
            Subcategory.Generic,
            ParamType.Material,
            false,
            true );
    }
}
