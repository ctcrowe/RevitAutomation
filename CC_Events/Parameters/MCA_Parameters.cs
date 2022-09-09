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
            public static readonly Param Height = new Param
                ("MCA_Height",
                 new Guid("2f97b5172f0b42888be2d20d2c817d26"),
                 Subcategory.Generic,
                 ParamType.Length,
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
            public static readonly Param PanelFinish = new Param
                ("MCA_Panel Finish",
                 new Guid("0ba77303a40142b9a39a653b79980816"),
                 Subcategory.Door,
                 ParamType.Material,
                 true,
                 true);
            public static readonly Param PanelMaterial = new Param
                ("MCA_Panel Material",
                 new Guid("7062a2d916594b7ab89d10d2ccee2bec"),
                 Subcategory.Door,
                 ParamType.Text,
                 true,
                 true);
            public static readonly Param PanelType = new Param
                ("MCA_Panel Type",
                 new Guid("d5913761461548d8bb089c6a7b7b96a5"),
                 Subcategory.Door,
                 ParamType.Text,
                 true,
                 true);
            public static readonly Param ProjectNo = new Param
                ("MCA_Project Number",
                 new Guid("1f8ba733904c4954945bd5226059001e"),
                 Subcategory.Project,
                 ParamType.Text,
                 true,
                 true);
            public static readonly Param SignOffDate = new Param
                ("MCA_Sheet Sign Off",
                 new Guid("66e7e146a5d54b04aadd9b6877d830e9"),
                 Subcategory.Sheet,
                 ParamType.Text,
                 true,
                 true);
            public static readonly Param StudSize = new Param
                ("MCA_Stud Size",
                 new Guid(""),
                 Subcategory.Wall,
                 ParamType.Text,
                 true,
                 true);
            public static readonly Param Width = new Param
                ("MCA_Width",
                 new Guid("c5c5ca08831e4c5bbfc33657e410641b),
                 Subcategory.Generic,
                 ParamType.Length,
                 true,
                 true);
        }
    }
}
