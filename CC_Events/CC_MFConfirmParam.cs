using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace CC_Plugin
{
    internal class MFConfirmParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "MasterFormat_Confirmed",
                    /* Type */              ParamType.Bool,
                    /* ID */                new Guid("98d1341d-cf2a-4701-8f3b-772496234477"),
                    /* Param Group */       "BIMData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_GenericModel},
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "CONFIRMS IF THE MASTERFORMAT DIVISION WAS SET AUTOMATICALLY OR THROUGH THE API.",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static void Add(Document doc) { RevitParamEdits.Add_FamilyParam(doc, P); }
        public static string Get(Document doc)
        {
            return P.Get(doc);
        }
        public static string Get(Family f)
        {
            return P.Get(f);
        }
        public static string Set(Document doc, int i)
        {
            Add(doc);
            return P.Set(doc, i.ToString());
        }
        private static Definition SetupParam(Document doc)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(P.ParamGroup) == null)
            {
                DefinitionGroup group = df.Groups.Create(P.ParamGroup);
                return group.Definitions.Create(new ExDefOptions(P).opt);
            }
            else
            {
                DefinitionGroup group = df.Groups.get_Item(P.ParamGroup);
                if (df.Groups.get_Item(P.ParamGroup).Definitions.get_Item(P.name) == null)
                {
                    return group.Definitions.Create(new ExDefOptions(P).opt);
                }
                else
                {
                    return group.Definitions.get_Item(P.name);
                }
            }
        }
    }
}