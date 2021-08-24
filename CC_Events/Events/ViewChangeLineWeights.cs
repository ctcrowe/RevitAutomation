using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;

using CC_Library;
using CC_Library.Predictions;
using CC_Library.Parameters;

namespace CC_Plugin.Events
{
    internal class ViewChangeLineWeights
    {
        public static void ViewChanged(object sender, ViewActivatedEventArgs args)
        {
            Document doc = args.CurrentActiveView.Document;
            LineWeights.Learn(doc);
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(ViewChanged);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(ViewChanged);
            return Result.Succeeded;
        }
    }
}
