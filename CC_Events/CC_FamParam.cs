using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
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
            
            if(doc.IsFamilyDocument)
            {
                using(Transaction t = new Transaction(doc, "Add Family"))
                {
                    t.Start();
                    Application app = doc.Application;
                    app.SharedParametersFilename = SharedParams;
                    DefinitionFile DefFile = app.OpenSharedParameterFile();
                    ExternalDefinition def = SetupParam(doc) as ExternalDefinition;
                    if (doc.FamilyManager.get_Parameter(P.ID) == null)
                        doc.FamilyManager.AddParameter(def, P.BuiltInGroup, false);
                    t.Commit();
                }
            }
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(Add);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(Add);
            return Result.Succeeded;
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
            Add(doc);
            return P.Set(doc, s);
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
