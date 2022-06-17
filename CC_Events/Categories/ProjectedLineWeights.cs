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
            var predset = ProjectionLineWeightNetwork.Predict(cat.Name, CMDLibrary.WriteNull);
            var pred = predset.ToList().IndexOf(predset.Max());
            typeof(ProjectionLineWeightNetwork).CreateEmbed(cat.Name, pred.ToString());
            cat.SetLineWeight(pred + 1, GraphicsStyleType.Projection);
        }
    }
}
