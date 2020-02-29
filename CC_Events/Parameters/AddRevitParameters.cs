using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class AddRevitParams
    {
        public static void AddFamilyParam(this<t> p, Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, p.Inst);
                }
            }
        }
        public static void AddProjectParam(this Param p, Document doc)
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
        public static void AddSpaceParam(this Param p, Document doc)
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
        public static void AddWallParam(this Param p, Document doc)
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
                        if (p.Inst)
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
        private static Definition CreateDefinition(this Param p, Document doc)
        {
            string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string SharedParams = Location + "\\CC_SharedParams.txt";

            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(p.ParamGroup) == null)
            {
                DefinitionGroup newgroup = df.Groups.Create(p.ParamGroup);
                if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
                {
                    return newgroup.Definitions.Create(p.CreateOptions());
                }
                else
                {
                    return newgroup.Definitions.get_Item(p.name);
                }
            }
            DefinitionGroup group = df.Groups.get_Item(p.ParamGroup);
            if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
            {
                return group.Definitions.Create(p.CreateOptions());
            }
            else
            {
                return group.Definitions.get_Item(p.name);
            }
        }
    }
}