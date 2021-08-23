using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace CC_Plugin
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
                            try
                            {
                                var ovrride = v.GetCategoryOverrides(cs.Id);
                                string name1 = cs.Name;
                                string name2 = v.Title;
                                if(cs.GetLineWeight(GraphicsStyleType.Cut) != null)
                                {
                                    int CutLineWeight = ovrride.CutLineWeight == OverrideGraphicSettings.InvalidPenNumber?
                                        (int)cs.GetLineWeight(GraphicsStyleType.Cut) :
                                        ovrride.CutLineWeight;
                                    Datatype.CutLineWeight.PropogateSingle(CutLineWeight, new WriteToCMDLine(WriteNull), name1, name2);
                                }
                                if(cs.GetLineWeight(GraphicsStyleType.Projection) != null)
                                {
                                    int ProjectedLineWeight = ovrride.ProjectionLineWeight ==OverrideGraphicSettings.InvalidPenNumber?
                                        (int) cs.GetLineWeight(GraphicsStyleType.Projection):
                                        ovrride.ProjectionLineWeight;
                                    Datatype.ProjectedLineWeight.PropogateSingle(ProjectedLineWeight, new WriteToCMDLine(WriteNull), name1, name2);
                                }
                            }
                            catch(Exception e) {e.OutputError();}
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
