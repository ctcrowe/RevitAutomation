using System;
using Autodesk.Revit.DB;
using CC_Library;

namespace CC_Plugin
{
    public static class CCAddCategories
    {
        public static void AddCategories(this Document doc)
        {
            if(doc.IsFamilyDocument)
            {
                Category cat = Category.GetCategory(doc, doc.OwnerFamily.FamilyCategoryId);
                var values = Enum.GetNames(typeof(ObjectCategory));
                foreach(string s in values)
                {
                    Category subcat;
                    if (!cat.SubCategories.Contains(s))
                    {
                        subcat = doc.Settings.Categories.NewSubcategory(cat, s);
                    }
                }
            }
        }
    }
}
