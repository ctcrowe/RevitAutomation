using Autodesk.Revit.DB;
using System;

namespace CC_Plugin
{
    internal class FamParam
    {
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
        public static string Get(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                if (String.IsNullOrEmpty(FamParam.P.Get(doc)))
                {
                    FamParam.P.Add(doc);
                    return null;
                }
                else
                {
                    return FamParam.P.Get(doc);
                }
            }
            return null;
        }
        public static string Set(Document doc, string S)
        {
            if (doc.IsFamilyDocument)
            {
                if (String.IsNullOrEmpty(FamParam.P.Get(doc)))
                {
                    FamParam.P.Add(doc);
                }
                return FamParam.P.Set(doc, S);
            }
            return null;
        }
    }
}