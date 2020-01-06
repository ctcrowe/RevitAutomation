using Autodesk.Revit.DB;
using System;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{
    internal class IDParam
    {
        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "ID",
                    /* Type */              ParamType.String,
                    /* ID */                new Guid("dc2385d1-4c41-4a81-be07-834d54ed32a6"),
                    /* Param Group */       "ID",
                    /* Categories */        new BuiltInCategory[2] {
                                                BuiltInCategory.OST_ProjectInformation,
                                                BuiltInCategory.OST_Rooms},
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "AN ID REFERENCE FOR DATA TRACKING",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static void Add(Document doc)
        {
            P.Add(doc);
        }
        public static string Check(Document doc)
        {
            if (!String.IsNullOrEmpty(Get(doc)))
                return Get(doc);
            return Set(doc);
        }
        public static string Check(Room r)
        {
            if (!String.IsNullOrEmpty(Get(r)))
                return Get(r);
            return Set(r);
        }
        public static string Get(Document doc)
        {
            return P.Get(doc);
        }
        public static string Get(Family f)
        {
            return P.Get(f);
        }
        public static string Get(Room r)
        {
            return P.Get(r);
        }
        public static string Get(FamilyInstance e)
        {
            return P.Get(e);
        }
        public static string Get(Element e)
        {
            return P.Get(e);
        }
        public static string Set(Document doc)
        {
            P.Add(doc);
            return P.Set(doc, Guid.NewGuid().ToString("N"));
        }
        public static string Set(Room r)
        {
            IDParam.P.Add(r.Document);
            return P.Set(r, Guid.NewGuid().ToString("N"));
        }
    }
}