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

        public static void LoadEvent(object sender, FamilyLoadedIntoDocumentEventArgs args)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string subdir = dir + "\\" + args.Document.Application.VersionNumber.ToString();
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);
            string fam = args.FamilyPath;
            string famfile = fam + args.FamilyName + ".rfa";
            if (!string.IsNullOrEmpty(args.FamilyName))
            {
                string fn = subdir + "\\" + args.FamilyName + ".rfa";
                int i = CheckUse(fn);
                if (i == 1)
                {
                    if(fn != famfile)
                    {
                        File.Delete(fn);
                        File.Copy(famfile, fn);
                    }
                }
                if(i == 0)
                {
                    File.Copy(famfile, fn);
                }
                if (!args.Document.IsFamilyDocument)
                {
                    string FilePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(args.Document.GetWorksharingCentralModelPath());
                    string dirpath = FilePath.TrimEnd(FilePath.Split('\\').LastOrDefault().ToCharArray());
                    string fullpath = dirpath + "\\ProjectFamilies";
                    if (!Directory.Exists(fullpath))
                        Directory.CreateDirectory(fullpath);
                    string fullfile = fullpath + "\\" + args.FamilyName + ".rfa";
                    if(File.Exists(fullfile) && famfile != fullfile)
                        File.Delete(fullfile);
                    File.Copy(famfile, fullfile);
                }
            }
        }
        public static int CheckUse(string famname)
        {
            if(File.Exists(famname))
            {
                TaskDialog d = new TaskDialog("File Exists!");
                d.MainInstruction = "The File Exists!";
                d.MainContent = "The family '" + famname.Split('\\').Last() + "' already exists! Would you like to replace it?";
                d.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Yes");
                d.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "No");
                d.CommonButtons = TaskDialogCommonButtons.Close;
                d.DefaultButton = TaskDialogResult.Close;
                
                TaskDialogResult tResult = d.Show();
                
                if (TaskDialogResult.CommandLink1 == tResult)
                {
                    return 1;
                }
                else
                    return 2;
            }
            return 0;
        }
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
    }
}
