using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;

namespace CC_Plugin
{
    public class RoomCategoryParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "RoomCategory",
                    /* Type */              ParamType.String,
                    /* ID */                new Guid("3231dffc-7322-43b6-9ede-d4e5fbb80399"),
                    /* Param Group */       "BIMData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_GenericModel},
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A REFERENCE TO THE USE OF THE ROOM.",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Guid ID { get { return P.ID; } }
        public static void Add(Document doc) { RevitParamEdits.Add_FamilyParam(doc, P); }
        public static string Get(Document doc)
        {
            return P.Get(doc);
        }
        public static string Get(Family f)
        {
            return P.Get(f);
        }
        public static string Get(FamilyInstance e)
        {
            return P.Get(e);
        }
        public static string Set(Document doc, string i)
        {
            return P.Set(doc, i);
        }
    }
}
