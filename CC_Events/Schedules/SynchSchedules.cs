using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System.IO;
using System.Linq;

namespace CC_Plugin.Schedules
{
    public class SynchSchedules
    {
        /*
        public static void Event(object sender, DocumentSynchronizedWithCentralEventArgs args)
        {
            if (!args.Document.IsFamilyDocument)
            {
                string FilePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(args.Document.GetWorksharingCentralModelPath());
                string dirpath = FilePath.TrimEnd(FilePath.Split('\\').LastOrDefault().ToCharArray());
                string fn = FilePath.Split('\\').Last().Split('.').First();
                string fullpath = dirpath + "\\Schedules";
                if (!Directory.Exists(fullpath))
                    Directory.CreateDirectory(fullpath);
                string DoorSched = fullpath + "\\" + fn + "_DoorSchedule.txt";
                DoorSchedule.Export(args.Document, DoorSched);
                string MatSched = fullpath + "\\" + fn + "_MaterialSchedule.txt";
                MaterialSchedule.Export(args.Document, MatSched);
            }
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSynchronizedWithCentral += new EventHandler<DocumentSynchronizedWithCentralEventArgs>(Event);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSynchronizedWithCentral -= new EventHandler<DocumentSynchronizedWithCentralEventArgs>(Event);
            return Result.Succeeded;
        }
        */
    }
}
