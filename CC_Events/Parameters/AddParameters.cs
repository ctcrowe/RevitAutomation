using System;
using System.Collections.Generic;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

using CC_Library.Parameters;
using CC_Library;

namespace CC_Plugin.Parameters
{
    public static class AddParameters
    {
        private const string Group = "Automatic";
        private static List<Category> Categories(this Document doc, Param p)
        {
            List<Category> cats = new List<Category>();
            switch (p.Subcategory)
            {
                default:
                case Subcategory.Doors:
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Doors));
                    break;
                case Subcategory.Generic:
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_GenericModel));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Doors));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallPanels));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Windows));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Casework));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_DetailComponents));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Furniture));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_FurnitureSystems));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_PlumbingFixtures));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_SpecialityEquipment));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_MechanicalEquipment));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_LightingFixtures));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_ElectricalEquipment));
                    break;
                case Subcategory.Project:
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_ProjectInformation));
                    break;
                case Subcategory.Materials:
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Materials));
                    break;
                case Subcategory.Rooms:
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Rooms));
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Areas));
                    break;
                case Subcategory.View:
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Views));
                    break;
                case Subcategory.Wall:
                    cats.Add(Category.GetCategory(doc, BuiltInCategory.OST_Walls));
                    break;
            }
            return cats;
        }
        internal static List<Param> GetParams()
        {
            List<Param> Params = new List<Param>();
            List<Type> Types = new List<Type>()
            {
                typeof(ProjectParams),
                typeof(RoomParams),
                typeof(DoorParams),
                typeof(MaterialParams),
                typeof(WallParams),
                typeof(SpecialParams)
            };
            foreach (var t in Types)
                foreach (var Field in t.GetFields())
                    Params.Add(Field.GetValue(null) as Param);
            return Params;
        }
        public static void AddParams(this Document doc)
        {
            ResetParamLibrary.Run();
            using (TransactionGroup tg = new TransactionGroup(doc, "Add Params"))
            {
                tg.Start();
                using (Transaction t = new Transaction(doc, "ADD PARAMS"))
                {
                    t.Start();
                    foreach (Param p in GetParams())
                        doc.AddParam(p);
                    t.Commit();
                }
                tg.Commit();
            }
        }
        public static void AddParam(this Document doc, Param p)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.Guid) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, p.Instance);
                }
            }
            else
            {
                CategorySet set = new CategorySet();
                foreach (var cat in doc.Categories(p))
                    set.Insert(cat);
                Definition def = p.CreateDefinition(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    if (p.Instance)
                        doc.ParameterBindings.Insert(def, new InstanceBinding(set));
                    else
                        doc.ParameterBindings.Insert(def, new TypeBinding(set));
                }
                else
                {
                    try
                    {
                        if (p.Instance)
                        {
                            var binding = doc.ParameterBindings.get_Item(def) as InstanceBinding;
                            if (binding != null)
                            {
                                foreach (Category cat in binding.Categories)
                                    if (!set.Contains(cat))
                                        set.Insert(cat);
                                doc.ParameterBindings.ReInsert(def, new InstanceBinding(set));
                            }
                        }
                        else
                        {
                            var binding = doc.ParameterBindings.get_Item(def) as TypeBinding;
                            if (binding != null)
                            {
                                foreach (Category cat in binding.Categories)
                                    if (!set.Contains(cat))
                                        set.Insert(cat);
                                doc.ParameterBindings.ReInsert(def, new TypeBinding(set));
                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}