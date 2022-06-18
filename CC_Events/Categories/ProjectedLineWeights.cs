using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using CC_Library;
using CC_Library.Predictions;

namespace CC_Plugin
{
    internal static class ProjectedLineWeights
    {
        public static void SetProjectedLineWeight(this Category cat)
        {
            var ProjectionPredset = ProjectionLineWeightNetwork.Predict(cat.Name, CMDLibrary.WriteNull);
            var ProjectionPred = ProjectionPredset.ToList().IndexOf(ProjectionPredset.Max());
            
            typeof(ProjectionLineWeightNetwork).CreateEmbed(cat.Name, ProjectionPred.ToString());
            cat.SetLineWeight(ProjectionPred + 1, GraphicsStyleType.Projection);
            
            if(cat.IsCuttable)
            {
                var CutPredset = CutLineWeightNetwork.Predict(cat.Name, CMDLibrary.WriteNull);
                var CutPred = CutPredset.ToList().IndexOf(CutPredset.Max());
                
                typeof(CutLineWeightNetwork).CreateEmbed(cat.Name, CutPred.ToString());
                cat.SetLineWeight(CutPred + 1, GraphicsStyleType.Cut);
            }
        }
    }
}


/*

step 1: collect view name
step 2: interpret view name in transformer

step 3: for each category in view, collect all subcategories
step 4: for each subcategory, intrepret name in transformer
step 5: merge view name output with subcategory output.
step 6: run output through extrapolator
step 7: use extrapolation output as vg override settings.
note: relate extrapolation for projection, cut, and visibility

*/
