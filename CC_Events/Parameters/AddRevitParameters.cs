using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class AddRevitParams
    {
        public static void AddParams(Document doc, IDParam id)
        {
            Foreach(var p in ...)
            {
                using(Transaction t = new Transaction(doc, "Add Params"))
                {
                    t.Start();
                    try
                    {
                    switch(p.Location)
                    {
                        case ParamLocation....
                            break;
                        case ParanLocation....
                            break;
                        case ParamLocation....
                            break;
                        case ParamLocation....
                            break;
                    }
                    t.Commit();
                    }
                    catch { t.RollBack(); }
                }
            }
        }
        public static void AddComboParam<t>(this t p, Document doc) where t : Param
        {
            if (doc.IsFamilyDocument)
                p.AddFamilyParam(doc);
            else
                p.AddProjectParam(doc);
        }
        public static void AddFamilyParam<t>(this t p, Document doc) where t : Param
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, p.IsInstance);
                }
            }
        }
        public static void AddProjectParam<t>(this t p, Document doc) where t : Param
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_ProjectInformation));
                        InstanceBinding binding = new InstanceBinding(set);
                        doc.ParameterBindings.Insert(def, binding);
                    }
                    catch { }
                }
            }
        }
        public static void AddSpaceParam<t>(this t p, Document doc) where t : Param
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Rooms));
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Areas));
                        InstanceBinding binding = new InstanceBinding(set);
                        doc.ParameterBindings.Insert(def, binding);
                    }
                    catch { }
                }
            }
        }
        public static void AddWallParam<t>(this t p, Document doc) where t : Param
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Walls));
                        if (p.IsInstance)
                        {
                            InstanceBinding binding = new InstanceBinding(set);
                            doc.ParameterBindings.Insert(def, binding);
                        }
                        else
                        {
                            TypeBinding binding = new TypeBinding(set);
                            doc.ParameterBindings.Insert(def, binding);
                        }
                    }
                    catch { }
                }
            }
        }
        private static Definition CreateDefinition<t>(this t p, Document doc) where t : Param
        {
            string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string SharedParams = Location + "\\CC_SharedParams.txt";

            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(p.Group) == null)
            {
                DefinitionGroup newgroup = df.Groups.Create(p.Group);
                if (df.Groups.get_Item(p.Group).Definitions.get_Item(p.Name) == null)
                {
                    return newgroup.Definitions.Create(p.CreateOptions());
                }
                else
                {
                    return newgroup.Definitions.get_Item(p.Name);
                }
            }
            DefinitionGroup group = df.Groups.get_Item(p.Group);
            if (df.Groups.get_Item(p.Group).Definitions.get_Item(p.Name) == null)
            {
                return group.Definitions.Create(p.CreateOptions());
            }
            else
            {
                return group.Definitions.get_Item(p.Name);
            }
        }
    }
}
