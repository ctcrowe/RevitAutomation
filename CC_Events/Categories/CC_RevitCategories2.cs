using CC_CoreData;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Events
{
    class RevitCategories
    {
        public static void RunCategorySetup(Document currentDoc)
        {
            RevitCategories cats = new RevitCategories(currentDoc);
            if (!currentDoc.IsFamilyDocument)
            {
                CategoryMethod method = new CategoryMethod(cats.CategorySetup);
                int i = CategoryCalls.CategoryLoop(method);
                TaskDialog.Show("Categories Setup", "The line weights have been set for " + i + " Categories!");
            }
            else
            {
                CategoryMethod method = new CategoryMethod(cats.CategorySetup);
                CategoryQualifier qual = new CategoryQualifier(cats.CategoryCheck);
                int i = CategoryCalls.CategoryLoop(method, qual);
                TaskDialog.Show("Categories Setup", "The line weights have been set for " + i + " Categories!");
            }
        }
        private Document currentDoc { get; }
        private RevitCategories(Document currentdoc)
        {
            this.currentDoc = currentdoc;
        }
        private bool CategoryCheck(CC_Category cat)
        {
            Category famcat = currentDoc.OwnerFamily.FamilyCategory;
            CC_Category ccfamcat = RevitCategories.getCC_Category(famcat);
            if (ccfamcat.Children.Contains(cat.Name) || cat.Parent == ccfamcat.Name)
                return true;
            return false;
        }
        private void CategorySetup(CC_Category cat)
        {
            if(cat.BuiltInCategory > 0)
            {
                Category bic = GetBuiltInCategory(currentDoc, cat.BuiltInCategory);
                setCategoryStyles(bic, cat);
            }
            else
            {
                CC_Category parentcat = CategoryCalls.getCategory(cat.Name);
                Category parent = GetBuiltInCategory(currentDoc, parentcat.BuiltInCategory);
                CategoryNameMap map = parent.SubCategories;
                if (!map.Contains(cat.Name))
                {
                    Category subcat = currentDoc.Settings.Categories.NewSubcategory(parent, cat.Name);
                    setCategoryStyles(subcat, cat);
                }
                else
                {
                    Category subcat = map.get_Item(cat.Name);
                    setCategoryStyles(subcat, cat);
                }
            }
        }
        private static void setCategoryStyles(Category cat, CC_Category cccat)
        {
            if (cccat.CutLineweight > 0)
                cat.SetLineWeight(cccat.CutLineweight, GraphicsStyleType.Cut);
            if (cccat.ProjLineweight > 0)
                cat.SetLineWeight(cccat.ProjLineweight, GraphicsStyleType.Projection);
            cat.LineColor = new Color(cccat.Color[0], cccat.Color[1], cccat.Color[2]);
            cat.Material = null;
        }
        private static Category GetBuiltInCategory(Document currentDoc, int category)
        {
            switch (category)
            {
                case BuiltinCategories.Walls:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Walls);
                case BuiltinCategories.Windows:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Windows);
                case BuiltinCategories.Doors:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Doors);
                case BuiltinCategories.Casework:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Casework);
                case BuiltinCategories.Ceilings:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Ceilings);
                case BuiltinCategories.Floors:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Floors);
                case BuiltinCategories.Furniture:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Furniture);
                case BuiltinCategories.FurnitureSystems:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_FurnitureSystems);
                case BuiltinCategories.GenericModels:
                default:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_GenericModel);
                case BuiltinCategories.Parts:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Parts);
                case BuiltinCategories.Roofs:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_Roofs);
                case BuiltinCategories.SpecialtyEquipment:
                    return Category.GetCategory(currentDoc, BuiltInCategory.OST_SpecialityEquipment);
            }
        }
        private static CC_Category getCC_Category(Category category)
        {
            switch(category.Id.IntegerValue)
            {
                case (int)BuiltInCategory.OST_Walls:
                    return CategoryLibrary.Walls;
                case (int)BuiltInCategory.OST_Windows:
                    return CategoryLibrary.Windows;
                case (int)BuiltInCategory.OST_Doors:
                    return CategoryLibrary.Doors;
                case (int)BuiltInCategory.OST_Casework:
                    return CategoryLibrary.Casework;
                case (int)BuiltInCategory.OST_Ceilings:
                    return CategoryLibrary.Ceilings;
                case (int)BuiltInCategory.OST_Floors:
                    return CategoryLibrary.Floors;
                case (int)BuiltInCategory.OST_Furniture:
                    return CategoryLibrary.Furniture;
                case (int)BuiltInCategory.OST_FurnitureSystems:
                    return CategoryLibrary.FurnitureSystems;
                case (int)BuiltInCategory.OST_GenericModel:
                default:
                    return CategoryLibrary.GenericModels;
                case (int)BuiltInCategory.OST_Parts:
                    return CategoryLibrary.Parts;
                case (int)BuiltInCategory.OST_Roofs:
                    return CategoryLibrary.Roofs;
                case (int)BuiltInCategory.OST_SpecialityEquipment:
                    return CategoryLibrary.SpecialtyEquipment;
            }
        }
    }
}
