using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Reflection;

namespace CC_LibraryUpdates
{
    public class CC_LibraryUpdates : IExternalApplication
    {
        private static string dllpath = Assembly.GetExecutingAssembly().Location;
        private const string TabName = "CCrowe";
        private const string PanelName = "Library Updates";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            try { uiApp.CreateRibbonTab(TabName); } catch { };
            RibbonPanel Panel = uiApp.CreateRibbonPanel(TabName, PanelName);
            PushButtonData b1d = new PushButtonData(
                "Update Library",
                "Update Library",
                @dllpath,
                "CC_LibraryUpdates.UpdateLibrary");
            b1d.ToolTip = "Create a reference border for the active detail view.";

            var item = Panel.AddItem(b1d) as PushButton;

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class UpdateLibrary : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a text file",
                Title = "Open txt file"
            })
            {

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fn = ofd.FileName;
                    string dir = Path.GetDirectoryName(fn);
                    var Files = Directory.GetFiles(dir);

                    foreach (var f in Files)
                    {
                        if (f.EndsWith(".rfa"))
                        {
                            var uidoc = commandData.Application.OpenAndActivateDocument(f);
                            uidoc.Document.Save();
                        }
                    }
                }
                return Result.Succeeded;
            }
        }
    }
}
