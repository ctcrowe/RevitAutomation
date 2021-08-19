using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CC_Plugin.Events
{
    public class LineWeights
    {
        public static void Learn(Document doc)
        {
            View v = doc.ActiveView;
            Categories cats = doc.Settings.Categories;
            foreach (Category c in cats)
            {
                if (c.CategoryType == CategoryType.Model)
                {
                    if (!c.SubCategories.IsEmpty)
                    {
                        foreach (Category cs in c.SubCategories)
                        {
                            var override = v.GetCategoryOverrides(cs.Id);
                            int CutLineWeight;
                            int ProjectedLineWeight;
                            if(override.CutLineWeight == OverrideGraphicSettings.InvalidPenNumber)
                            {
                                
                            }
                            else
                            {
                            }
                            if(override.ProjectionLineWeight == OvverideGraphicSettings.InvalidPenNumber)
                            {
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
        }
    }
}
