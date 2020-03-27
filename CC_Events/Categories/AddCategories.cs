using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
        public static List<string> GetCategories(this FamilyInstance inst)
        {
            List<string> categories = new List<string>();
            try
            {
                GeometryElement geoElem = inst.get_Geometry(new Options());


                foreach (GeometryObject obj in geoElem)
                {
                    try
                    {
                        if (obj is GeometryInstance)
                        {
                            GeometryInstance geoInst
                              = obj as GeometryInstance;

                            GeometryElement geoElem2
                              = geoInst.GetSymbolGeometry();

                            try
                            {
                                foreach (GeometryObject geoObj2 in geoElem2)
                                {
                                    if (geoObj2 is Solid)
                                    {
                                        Solid solid = geoObj2 as Solid;

                                        ElementId id = solid.GraphicsStyleId;

                                        GraphicsStyle gStyle = inst.Document.GetElement(id) as GraphicsStyle;

                                        if (gStyle != null)
                                        {
                                            try
                                            {
                                                categories.Add(gStyle.GraphicsStyleCategory.Name.SimplifyTitle());
                                            }
                                            catch
                                            { TaskDialog.Show("Error", "Failed at Add Style"); }
                                        }
                                    }
                                }
                            }
                            catch
                            { TaskDialog.Show("Error", "Failed at Second Break"); }
                        }
                    }
                    catch
                    { TaskDialog.Show("Error", "Failed at First Break"); }
                }
            }
            catch { TaskDialog.Show("Error", "Failed To Collect Geometry"); }
            return categories;
        }
    }
}
