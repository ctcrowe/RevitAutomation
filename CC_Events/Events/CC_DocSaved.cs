using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
namespace CC_Plugin
{
    class DocumentSaved
    {
        public static void SavedEvent(object sender, DocumentSavedEventArgs args)
        {
            MFSaveFamily.Main(args.Document);
        }
        public static void SavedAsEvent(object sender, DocumentSavedAsEventArgs args)
        {
            MFSaveFamily.Main(args.Document);
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSaved += new EventHandler<DocumentSavedEventArgs>(SavedEvent);
            app.ControlledApplication.DocumentSavedAs += new EventHandler<DocumentSavedAsEventArgs>(SavedAsEvent);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSaved -= new EventHandler<DocumentSavedEventArgs>(SavedEvent);
            app.ControlledApplication.DocumentSavedAs -= new EventHandler<DocumentSavedAsEventArgs>(SavedAsEvent);
            return Result.Succeeded;
        }
    }
}
