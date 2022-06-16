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
        { cat.SetLineWeight(
            ProjectionLineWeightNetwork.Predict(cat.Name, CMDLibrary.WriteNull),
            GraphicsStyleType.Projection);
        }
    }
}
