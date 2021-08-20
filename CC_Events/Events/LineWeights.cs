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
                            int CutLineWeight;
                            int ProjectedLineWeight;
                            if(ovrride.CutLineWeight == OverrideGraphicSettings.InvalidPenNumber && cs.GetLineWeight(GraphicsStyleType.Cut) != null)
                            {
                                CutLineWeight = (int)cs.GetLineWeight(GraphicsStyleType.Cut);
                            }
                            else
                            {
                                CutLineWeight = ovrride.CutLineWeight;
                            }
                            if(ovrride.ProjectionLineWeight == OverrideGraphicSettings.InvalidPenNumber && cs.GetLineWeight(GraphicsStyleType.Cut) != null)
                            {
                                ProjectedLineWeight = (int)cs.GetLineWeight(GraphicsStyleType.Projection);
                            }
                            else
                            {
                                ProjectedLineWeight = ovrride.ProjectionLineWeight;
                            }
                            string name1 = cs.Name;
                            string name2 = v.Title;
                            if(cs.GetLineWeight(GraphicsStyleType.Cut) != null)
                                Datatype.CutLineWeight.PropogateSingle(CutLineWeight, new WriteToCMDLine(WriteNull), name1, name2);
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