using System;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    internal class DocSynching
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSynchronizingWithCentral += new EventHandler<DocumentSynchronizingWithCentralEventArgs>(synch);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSynchronizingWithCentral -= new EventHandler<DocumentSynchronizingWithCentralEventArgs>(synch);
            return Result.Succeeded;
        }
        public static void synch(object sender, DocumentSynchronizingWithCentralEventArgs args)
        {
            Document doc = args.Document;
            using (Transaction t = new Transaction(doc, "Add Families"))
            {
                t.Start();
                EmbeddedFamilies.run(doc, "Symbols");
                t.Commit();
            }
        }
    }
}
