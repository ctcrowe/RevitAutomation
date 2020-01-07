using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;


namespace CC_Plugin
{
    internal class IDParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

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
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile DefFile = app.OpenSharedParameterFile();
            if (doc.IsFamilyDocument)
            {
                ExternalDefinition def = SetupParam(doc) as ExternalDefinition;
                if (doc.FamilyManager.get_Parameter(P.ID) == null)
                    doc.FamilyManager.AddParameter(def, P.BuiltInGroup, false);
            }
            else
            {
                Definition def = SetupParam(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        foreach (BuiltInCategory cat in P.Categories)
                        {
                            if (!set.Contains(Category.GetCategory(doc, cat)))
                                set.Insert(Category.GetCategory(doc, cat));
                        }
                        if (set.Size > 0)
                        {
                            InstanceBinding binding = new InstanceBinding(set);
                            doc.ParameterBindings.Insert(def, binding);
                        }
                    }
                    catch { }
                }
            }
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
            Add(doc);
            return P.Set(doc, Guid.NewGuid().ToString("N"));
        }
        public static string Set(Room r)
        {
            IDParam.P.Add(r.Document);
            return P.Set(r, Guid.NewGuid().ToString("N"));
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