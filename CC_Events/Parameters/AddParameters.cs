using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class AddParameters
    {
        private static Definition CreateDefinition(this CCParameter p, Document doc)
        {
            string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string SharedParams = Location + "\\CC_SharedParams.txt";
            ResetParamLibrary.Run();

            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item("Automatic") == null)
            {
                DefinitionGroup newgroup = df.Groups.Create("Automatic");
                if (df.Groups.get_Item("Automatic").Definitions.get_Item(p.ToString()) == null)
                {
                    return newgroup.Definitions.Create(p.CreateOptions());
                }
                else
                {
                    return newgroup.Definitions.get_Item(p.ToString());
                }
            }
            DefinitionGroup group = df.Groups.get_Item("Automatic");
            return group.Definitions.Create(p.CreateOptions());
        }
        public static void AddParam(this Document doc, CCParameter p)
        {
            int value = (int)p;
            int ptype = Math.Abs((value / 1000) % 10);
            switch (ptype)
            {
                default:
                case 0:
                    doc.AddProjectInfoParam(p);
                    doc.AddFamilyParam(p); break;
                case 1:
                    doc.AddProjectInfoParam(p); break;
                case 2:
                    doc.AddFamilyParam(p); break;
                case 3:
                    doc.AddRoomParam(p); break;
                case 4:
                    doc.AddCFWParam(p); break;
            }
        }
        private static void AddProjectInfoParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                if (doc.ProjectInformation.get_Parameter(p.GetGUID()) == null)
                {
                    Definition def = p.CreateDefinition(doc);
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_ProjectInformation));
                        if ((int)p <= 0)
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
        private static void AddRoomParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                List<Element> e = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToList();
                if (e.Any())
                {
                    if (e.First().get_Parameter(p.GetGUID()) == null)
                    {
                        Definition def = p.CreateDefinition(doc);
                        try
                        {
                            CategorySet set = new CategorySet();
                            set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Rooms));
                            set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Areas));
                            if ((int)p < 0)
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
        private static void AddCFWParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                List<Element> e = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).ToList();
                if (e.First().get_Parameter(p.GetGUID()) == null)
                {
                    Definition def = p.CreateDefinition(doc);
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Ceilings));
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Walls));
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Floors));
                        if ((int)p < 0)
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
        private static void AddFamilyParam(this Document doc, CCParameter p)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.GetGUID()) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, (int)p < 0);
                }
            }
        }
    }
}
