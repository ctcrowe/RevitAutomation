using System;
using Autodesk.Revit.DB;
using CC_Library;
//https://adndevblog.typepad.com/aec/2012/05/accessing-subcategory-information-from-inplace-family-instances.html
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
    public static void GetCategories(this Element ele)
    {
        GeometryElement ge = ele.Geometry;
        foreach(GeometryObject go in ge)
        {
            if(go is GeometryInstance)
            {
            }
        }
    }
}
