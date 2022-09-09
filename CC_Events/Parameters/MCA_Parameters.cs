using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Parameters;

namespace CC_Plugin.Parameters
{
    internal class MCA_Parameters
    {
        public static class MCA_Params
        {
            public static readonly Param ProjectNo = new Param
                ("MCA_Project Number",
                 new Guid(),
                 Subcategory.Project,
                 ParamType.Text,
                 true,
                 true);
            public static readonly Param Masterformat = new Param
                ("MCA_MF Division",
                new Guid("aedbf7d2d73c44048cf7ed26f3231d25"),
                Subcategory.Generic,
                ParamType.Int,
                false,
                true);
            public static readonly Param Occupancy = new Param
                ("MCA_Occupancy Group",
                 new Guid("d9e989bf54144291ac599107f0bff748"),
                 Subcategory.Rooms,
                 ParamType.Text,
                 true,
                 true);
            public static readonly Param OccupantLoadFactor = new Param
                ("MCA_Occupant Load Factor",
                 new Guid("cd4aa818198e418dbef1ffae4492fb64"),
                 Subcategory.Rooms,
                 ParamType.Area,
                 true,
                 true);
            public static readonly Param Height = new Param
                ("MCA_Height",
                 new Guid("2f97b5172f0b42888be2d20d2c817d26"),
                 Subcategory.Generic,
                 ParamType.Length,
                 true,
                 true);
        }
    }
}
