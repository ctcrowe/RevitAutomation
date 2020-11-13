using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

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
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Schedules");

            PushButtonData pbd = new PushButtonData(
                "Door Schedule",
                "Door Schedule",
                @dllpath(),
                "CC_Plugin.CreateDoorSchedule");

            PushButton pb1 = CPanel.AddItem(pbd) as PushButton;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CreateDoorSchedule
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            DoorSchedule.CreateSched(doc);
            return Result.Succeeded;
        }
    }
}
