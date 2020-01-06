using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System.IO;

namespace CC_Plugin
{
    internal class FamLoadedEvent
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string dir = directory + "\\CC_ElesByID";

        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.FamilyLoadedIntoDocument += new EventHandler<FamilyLoadedIntoDocumentEventArgs>(LoadEvent);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.FamilyLoadedIntoDocument -= new EventHandler<FamilyLoadedIntoDocumentEventArgs>(LoadEvent);
            return Result.Succeeded;
        }

        public static void LoadEvent(object sender, FamilyLoadedIntoDocumentEventArgs args)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string fam = args.FamilyPath;
            ElementId eid = args.NewFamilyId;
            if (eid == null)
            {
                eid = args.OriginalFamilyId;
            }
            Family e = args.Document.GetElement(eid) as Family;
            string id = IDParam.Get(e);
            if (!string.IsNullOrEmpty(id))
            {
                string famfile = fam + args.FamilyName + ".rfa";
                string fn = dir + "\\" + id + ".rfa";
                if (File.Exists(fn))
                    File.Delete(fn);
                File.Copy(famfile, fn);
            }
        }
    }
}
