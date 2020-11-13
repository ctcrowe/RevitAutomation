using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace CC_Plugin
{
    public class AnalysisTab
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Plugin.dll";
            return dll;
        }
        public static void AnalysisPanel(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Analysis");

            ComboBoxData cbd = new ComboBoxData("Analysis Tools");

            ComboBox cb = CPanel.AddItem(cbd) as ComboBox;
            cb.AddItem(new ComboBoxMemberData("Collect Load Factor Data", "Collect Load Factor Data"));
            cb.AddItem(new ComboBoxMemberData("Collect Family MF Data", "Collect Family MF Data"));

            PushButtonData b1Data = new PushButtonData(
                "Analysis Button",
                "Run Analysis",
                @dllpath(),
                "CC_Plugin.AnalysisButton");
            b1Data.ToolTip = "Run the current drop down analysis tool.";

            PushButton pb1 = CPanel.AddItem(b1Data) as PushButton;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class AnalysisButton : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var panels = uiapp.GetRibbonPanels(CCRibbon.tabName);
            var box = panels.Where(x => x.Name == "Analysis").First().GetItems().Where(x => x.Name == "Analysis Tools").First() as ComboBox;
            var current = box.Current.Name;
            switch(current)
            {
                default:
                case "Collect Load Factor Data":
                    uiapp.ActiveUIDocument.Document.CaptureLoadFactor();
                    break;
                case "Collect Family MF Data":
                    break;
            }
            return Result.Succeeded;
        }
    }
}
