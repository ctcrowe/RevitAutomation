using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

using CC_Library;
using CC_Plugin.Schedules;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace CC_Plugin
{
    public class SchduleTab
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Plugin.dll";
            return dll;
        }
        public static void SchedulePanel(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Objects");
            PushButtonData pbc = new PushButtonData(
                "Create Schedule",
                "Create Schedule",
                @dllpath(),
                "CC_Plugin.CreateSchedule");

            PushButtonData pbi = new PushButtonData(
                "Import Schedule",
                "Import Schedule",
                dllpath(),
                "CC_Plugin.ImportSchedule");

            PushButtonData pbo = new PushButtonData(
                "Import Objects",
                "Import Objects",
                dllpath(),
                "CC_Plugin.ImportObjects");

            ComboBoxData cbd = new ComboBoxData("Object Tools");

            IList<RibbonItem> Buttons = CPanel.AddStackedItems(pbc, pbi, pbo);
            ComboBox cb = CPanel.AddItem(cbd) as ComboBox;
            cb.AddItem(new ComboBoxMemberData("Doors", "Doors"));
            cb.AddItem(new ComboBoxMemberData("Materials", "Materials"));
        }
    }

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CreateSchedule : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var panels = uiapp.GetRibbonPanels(CCRibbon.tabName);
            var box = panels.Where(x => x.Name == "Objects").First().GetItems().Where(x => x.Name == "Object Tools").First() as ComboBox;
            var current = box.Current.Name;
            switch (current)
            {
                default:
                case "Doors":
                    DoorSchedule.CreateSched(uiapp.ActiveUIDocument.Document);
                    break;
                case "Materials":
                    MaterialSchedule.CreateSched(uiapp.ActiveUIDocument.Document);
                    break;
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ImportSchedule : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var panels = uiapp.GetRibbonPanels(CCRibbon.tabName);
            var box = panels.Where(x => x.Name == "Objects").First().GetItems().Where(x => x.Name == "Object Tools").First() as ComboBox;
            var current = box.Current.Name;
            var filepath = OpenFile.Run();
            switch (current)
            {
                default:
                case "Doors":
                    DoorSchedule.Import(uiapp.ActiveUIDocument.Document, filepath);
                    break;
                case "Materials":
                    MaterialSchedule.Import(uiapp.ActiveUIDocument.Document, filepath);
                    break;
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ImportObjects : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var panels = uiapp.GetRibbonPanels(CCRibbon.tabName);
            var box = panels.Where(x => x.Name == "Objects").First().GetItems().Where(x => x.Name == "Object Tools").First() as ComboBox;
            var current = box.Current.Name;
            switch (current)
            {
                default:
                case "Doors":
                    Assembly assembly = Assembly.LoadFrom("C:\\ProgramData\\Autodesk\\Revit\\Addins\\2019\\CCPlugin\\Doors.dll");
                    uiapp.ActiveUIDocument.Document.UnpackFamilies(assembly);
                    break;
                    /*
                case "Material Legend":
                    MaterialSchedule.Import(uiapp.ActiveUIDocument.Document, filepath);
                    break;
                    */
            }
            return Result.Succeeded;
        }
    }
}
