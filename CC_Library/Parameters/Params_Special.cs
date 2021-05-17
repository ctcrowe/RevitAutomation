using System;

namespace CC_Library.Parameters
{
    public static class SpecialParams
    {
        private static Subcategory sub = Subcategory.Generic;
        public static readonly Param MaterialInstance = new Param
            ("MaterialInstance",
            new Guid("f24a1aabfe4f4871bcc6103178a5337b"),
            sub,
            ParamType.Material,
            true);
    }
}
