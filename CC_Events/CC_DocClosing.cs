using System;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_Library.Parameters;
using LoadFamilies;

namespace CC_Plugin
{
    internal class DocClosing
    {

        public static void synch(object sender, DocumentClosingEventArgs args)
        {
            Document doc = args.Document;
        }

        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentClosing += new EventHandler<DocumentClosingEventArgs>(synch);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentClosing -= new EventHandler<DocumentClosingEventArgs>(synch);
            return Result.Succeeded;
        }
    }
}