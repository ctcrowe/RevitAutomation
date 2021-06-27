using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_Library;

namespace CC_Plugin
{
    public static class CC_AddCategories
    {
        public static Category AddCategories(this Document doc, int catnumber)
        {
            if (doc.IsFamilyDocument)
            {
                Category cat = Category.GetCategory(doc, doc.OwnerFamily.FamilyCategoryId);
                var values = Enum.GetNames(typeof(ObjectCategory));

                Category subcat;
                if (!cat.SubCategories.Contains(values[catnumber])) { subcat = doc.Settings.Categories.NewSubcategory(cat, values[catnumber]); }
                else { subcat = cat.SubCategories.get_Item(values[catnumber]); }

                return subcat;
            }
            else
                return null;
        }
    }
}
