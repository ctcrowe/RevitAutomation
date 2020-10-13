using System;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_Library.Parameters;
using LoadFamilies;

using CC_RevitBasics;

namespace CC_Plugin
{
    internal class DocSynching
    {

        public static void synch(object sender, DocumentSynchronizingWithCentralEventArgs args)
        {
            Document doc = args.Document;
            CCMethod.Run("DocSynching", "CC_DocSynching.DocSynching", "synch", doc);
        }
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string dir = directory + "\\CC_PrjData";

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
    }
}