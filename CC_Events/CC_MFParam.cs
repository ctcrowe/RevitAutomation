using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace CC_Plugin
{
    internal class MFParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "MasterFormat",
                    /* Type */              ParamType.String,
                    /* ID */                new Guid("98eefdd9-495d-4b4c-912b-aa7ce952b142"),
                    /* Param Group */       "BIMData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_GenericModel},
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A REFERENCE TO THE OBJECTS MASTERFORMAT DIVISION.",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Guid ID { get { return P.ID; } }
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
        public static string Set(Document doc, string i)
        {
            return P.Set(doc, i);
        }
    }
}
