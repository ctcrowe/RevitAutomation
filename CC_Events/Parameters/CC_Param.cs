using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;
using CC_Library;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class GetRevitParams
    {
        
    }
    internal static class SetRevitParams
    {
        public static void AddFamilyParam(this CC_Library.Parameters.Param p, Document doc)
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
        public static void AddProjectParam(this CC_Library.Parameters.Param p, Document doc)
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
        public static void AddSpaceParam(this CC_Library.Parameters.Param p, Document doc)
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
        public static void AddWallParam(this CC_Library.Parameters.Param p, Document doc)
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
    }
    internal class RevitParamEdits
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";
        
        public static void Add_FamilyParam(Document doc, Param p)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile DefFile = app.OpenSharedParameterFile();
            if (doc.IsFamilyDocument)
            {
                ExternalDefinition def = SetupParamDefinition(doc, p) as ExternalDefinition;
                if (doc.FamilyManager.get_Parameter(p.ID) == null)
                    doc.FamilyManager.AddParameter(def, p.BuiltInGroup, p.Inst);
            }
        }
        public static void Add_ProjectInfoParam(Document doc, Param p)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile DefFile = app.OpenSharedParameterFile();
            
            if(!doc.IsFamilyDocument)
            {
                Definition def = SetupParamDefinition(doc, p);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        foreach (BuiltInCategory cat in p.Categories)
                        {
                            if (!set.Contains(Category.GetCategory(doc, cat)))
                                set.Insert(Category.GetCategory(doc, cat));
                        }
                        if (set.Size > 0)
                        {
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
                    }
                    catch { }
                }
            }
        }
        public static bool Get_BooleanFamilyParam(Document doc, Param p)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(p.ID);
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        int i = t.AsInteger(param) ?? 0;
                        if(i == 1)
                            return true;
                    }
                }
            }
            return false;
        }
        private static Definition SetupParamDefinition(Document doc, Param p)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(p.ParamGroup) == null)
            {
                DefinitionGroup newgroup = df.Groups.Create(p.ParamGroup);
                if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
                {
                    return newgroup.Definitions.Create(new ExDefOptions(p).opt);
                }
                else
                {
                    return newgroup.Definitions.get_Item(p.name);
                }
            }
            DefinitionGroup group = df.Groups.get_Item(p.ParamGroup);
            if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
            {
                return group.Definitions.Create(new ExDefOptions(p).opt);
            }
            else
            {
                return group.Definitions.get_Item(p.name);
            }
        }
    }
}
