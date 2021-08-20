using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using CC_Library.Predictions;
using CC_Library.Datatypes;

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
                            var ovrride = v.GetCategoryOverrides(cs.Id);
                            string name1 = cs.Name;
                            string name2 = v.Title;
                            if(cs.GetLineWeight(GraphicsStyleType.Cut) != null)
                            {
                                int CutLineWeight = ovrride.CutLineWeight == OverrideGraphicsSettings.InvalidPenNumner?
                                    (int)cs.GetLineWeight(GraphicsStyleType.Cut) :
                                    ovrride.CutLineWeight;
                                if(cs.GetLineWeight(GraphicsStyleType.Cut) != null)
                                    Datatype.CutLineWeight.PropogateSingle(CutLineWeight, new WriteToCMDLine(WriteNull), name1, name2);
                            }
                            if(cs.GetLineWeight(GraphicsStyleType.Projection) != null)
                            {
                                int ProjectedLineWeight;
                            }

                            if(ovrride.ProjectionLineWeight == OverrideGraphicSettings.InvalidPenNumber && cs.GetLineWeight(GraphicsStyleType.Cut) != null)
                            {
                                ProjectedLineWeight = (int)cs.GetLineWeight(GraphicsStyleType.Projection);
                            }
                            else
                            {
                                ProjectedLineWeight = ovrride.ProjectionLineWeight;
                            }
                        }
                    }
                }
            }
        }
        public static string WriteNull(string s)
        {
            return s;
        }
    }
}
