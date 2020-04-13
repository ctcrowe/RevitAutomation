using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using CC_Library;

namespace CC_Plugin
{
    public static class VerifyCategories
    {
        public static void PurgeCategories(this Document doc)
        {
            Categories cats = doc.Settings.Categories;
            string[] categories = Enum.GetNames(typeof(ObjectCategory));
            foreach(Category c in cats)
            {
                if (c.CategoryType == CategoryType.Model)
                {
                    if (!c.SubCategories.IsEmpty)
                    {
                        foreach (Category cs in c.SubCategories)
                        {
                            using (Transaction t = new Transaction(doc, "DeleteCategory"))
                            {
                                t.Start();
                                if (!categories.Any(x => x == cs.Name))
                                {
                                    try { doc.Delete(cs.Id); } catch { }
                                }
                                t.Commit();
                            }
                        }
                    }
                }
            }
        }
    }
}