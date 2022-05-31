using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
namespace CC_Plugin
{
    class DocumentSaved
    {
        public static void SavingEvent(object sender, DocumentSavingEventArgs args)
        {
            using (TransactionGroup tg = new TransactionGroup(args.Document, "SaivngEvent"))
            {
                tg.Start();
                RevitCategories.ReviseCategories(args.Document);
                MFSaveFamily.SetFamily(args.Document.PathName, args.Document);
                tg.Commit();
            }
        }
        public static void SavingAsEvent(object sender, DocumentSavingAsEventArgs args)
        {
            using (TransactionGroup tg = new TransactionGroup(args.Document, "SaivngEvent"))
            {
                tg.Start();
                RevitCategories.ReviseCategories(args.Document);
                MFSaveFamily.SetFamily(args.Document.PathName, args.Document);
                tg.Commit();
            }
        }
        public static void SavedEvent(object sender, DocumentSavedEventArgs args)
        {
            MFSaveFamily.SaveFamily(args.Document.PathName, args.Document);
        }
        public static void SavedAsEvent(object sender, DocumentSavedAsEventArgs args)
        {
            MFSaveFamily.SaveFamily(args.Document.PathName, args.Document);
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSaved += new EventHandler<DocumentSavedEventArgs>(SavedEvent);
            app.ControlledApplication.DocumentSavedAs += new EventHandler<DocumentSavedAsEventArgs>(SavedAsEvent);
            app.ControlledApplication.DocumentSaving += new EventHandler<DocumentSavingEventArgs>(SavingEvent);
            app.ControlledApplication.DocumentSavingAs += new EventHandler<DocumentSavingAsEventArgs>(SavingAsEvent);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSaved -= new EventHandler<DocumentSavedEventArgs>(SavedEvent);
            app.ControlledApplication.DocumentSavedAs -= new EventHandler<DocumentSavedAsEventArgs>(SavedAsEvent);
            app.ControlledApplication.DocumentSaving -= new EventHandler<DocumentSavingEventArgs>(SavingEvent);
            app.ControlledApplication.DocumentSavingAs -= new EventHandler<DocumentSavingAsEventArgs>(SavingAsEvent);
            return Result.Succeeded;
        }
    }
}
