using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System.IO;
using System.Linq;

namespace CC_Plugin
{
    internal class FamLoadedEvent
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string dir = directory + "\\CC_ElesByID";

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
            string famfile = fam + args.FamilyName + ".rfa";
            if (!string.IsNullOrEmpty(id))
            {
                string fn = dir + "\\" + id + ".rfa";
                if (File.Exists(fn))
                    File.Delete(fn);
                File.Copy(famfile, fn);
            }
            if (!args.Document.IsFamilyDocument)
            {
                string FilePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(args.Document.GetWorksharingCentralModelPath());
                string dirpath = FilePath.TrimEnd(FilePath.Split('\\').LastOrDefault().ToCharArray());
                string fullpath = dirpath + "\\ProjectFamilies";
                if (!Directory.Exists(fullpath))
                    Directory.CreateDirectory(fullpath);
                File.Copy(famfile, fullpath + "\\" + id + ".rfa");
            }
        }
    }
}
