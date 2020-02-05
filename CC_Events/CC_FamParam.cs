using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;

namespace CC_Plugin
{
    internal class FamParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "FNAME",
                    /* Type */              ParamType.String,
                    /* ID */                new Guid("193b3ca2-da43-468f-adb2-3d8d4d300749"),
                    /* Param Group */       "ID",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_GenericModel},
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A FAMILY NAME REFERENCE FOR DATA TRACKING",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static void Add(object sender, ViewActivatedEventArgs args)
        {
            Document doc = args.Document;
            using(Transaction t = new Transaction(doc, "Add Family"))
            {
                t.Start();
                RevitParamEdits.Add_FamilyParam(doc, P);
                t.Commit();
            }
        }
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
        public static string Get(Element e)
        {
            return P.Get(e);
        }
        public static string Set(Document doc, string s)
        {
            return P.Set(doc, s);
        }
    }
}
