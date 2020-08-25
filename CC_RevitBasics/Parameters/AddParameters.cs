using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

using CC_Library.Parameters;

namespace CC_RevitBasics
{
    public static class AddParameters
    {
        private static Definition CreateDefinition(this CCParameter p, Document doc)
        {
            string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string SharedParams = Location + "\\CC_SharedParams.txt";

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
                    doc.AddComboParam(p); break;
                case 1:
                    doc.AddProjectInfoParam(p); break;
                case 2:
                    doc.AddFamilyParam(p); break;
                case 3:
                    doc.AddRoomParam(p); break;
                case 4:
                    doc.AddCFWParam(p); break;
                case 5:
                    doc.AddViewParam(p); break;
                case 6:
                    doc.AddCFWParam(p); break;
                case 7:
                    doc.AddOpeningParam(p); break;
                case 8:
                    doc.AddScopeParam(p); break;
                case 9:
                    doc.AddTotalParameter(p); break;
            }
        }
        private static void AddTotalParameter(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                CategorySet set = new CategorySet();
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Casework));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Ceilings));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Columns));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallPanels));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallMullions));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_DetailComponents));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Doors));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_ElectricalEquipment));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_ElectricalFixtures));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Floors));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Furniture));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_FurnitureSystems));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_GenericModel));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_LightingDevices));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_LightingFixtures));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_MechanicalEquipment));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PlumbingFixtures));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_StairsRailing));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Ramps));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_SpecialityEquipment));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Stairs));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Walls));
                set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Windows));
                InstanceBinding binding = new InstanceBinding(set);
                doc.ParameterBindings.Insert(def, binding);
            }
            else
            {
                if (doc.FamilyManager.get_Parameter(p.GetGUID()) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, (int)p < 0);
                }
            }
        }
        private static void AddScopeParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                try
                {
                    CategorySet set = new CategorySet();
                    set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Rooms));
                    set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Areas));
                    set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Ceilings));
                    set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Floors));
                    InstanceBinding binding = new InstanceBinding(set);
                    doc.ParameterBindings.Insert(def, binding);
                }
                catch { }
            }
        }
        private static void AddComboParam(this Document doc, CCParameter p)
        {
            if (doc.IsFamilyDocument)
                doc.AddFamilyParam(p);
            else
                doc.AddProjectInfoParam(p);
        }
        private static void AddProjectInfoParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                if (doc.ProjectInformation.get_Parameter(p.GetGUID()) == null)
                {
                    Definition def = p.CreateDefinition(doc);
                    CategorySet set = new CategorySet();
                    set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_ProjectInformation));
                    InstanceBinding binding = new InstanceBinding(set);
                    doc.ParameterBindings.Insert(def, binding);
                }
            }
        }
        private static void AddRoomParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
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
        private static void AddViewParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                try
                {
                    CategorySet set = new CategorySet();
                    set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Views));
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
        private static void AddOpeningParam(this Document doc, CCParameter p)
        {
            if (doc.IsFamilyDocument)
            {
                ElementId FamilyCategory = doc.OwnerFamily.FamilyCategoryId;
                List<ElementId> Categories = new List<ElementId>()
                {
                    Category.GetCategory(doc, BuiltInCategory.OST_Doors).Id,
                    Category.GetCategory(doc, BuiltInCategory.OST_Windows).Id,
                    Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallPanels).Id,
                    Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallMullions).Id
                };
                if (Categories.Any(x => x == FamilyCategory))
                {
                    if (doc.FamilyManager.get_Parameter(p.GetGUID()) == null)
                    {
                        ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                        doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, (int)p < 0);
                    }
                }
            }
            else
            {
                List<Element> e = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors).ToList();
                if (e.First().get_Parameter(p.GetGUID()) == null)
                {
                    Definition def = p.CreateDefinition(doc);
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Doors));
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Windows));
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallPanels));
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallMullions));
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
}
