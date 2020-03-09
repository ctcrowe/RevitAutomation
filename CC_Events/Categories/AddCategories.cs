using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_Library;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

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
        public static Dictionary<string, string> GetCategories(this Element ele)
        {
            GeometryElement geoElem = ele.get_Geometry(new Options());

            Dictionary<string, string> categories = new Dictionary<string, string>();

            FamilyInstance inst = ele as FamilyInstance;
            categories.Add("NAME", inst.Symbol.Family.Name);
            foreach (GeometryObject obj in geoElem)
            {
                if (obj is GeometryInstance)
                {
                    GeometryInstance geoInst
                      = obj as GeometryInstance;

                    GeometryElement geoElem2
                      = geoInst.GetSymbolGeometry();

                    foreach (GeometryObject geoObj2 in geoElem2)
                    {
                        if (geoObj2 is Solid)
                        {
                            Solid solid = geoObj2 as Solid;

                            ElementId id = solid.GraphicsStyleId;

                            GraphicsStyle gStyle = ele.Document.GetElement(id) as GraphicsStyle;

                            if (gStyle != null)
                            {
                                categories.Add(gStyle.GraphicsStyleCategory.Name, "1");
                            }
                        }
                    }
                }
            }
            return categories;
        }
    }
}
