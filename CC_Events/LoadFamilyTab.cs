using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

using CC_Plugin.Parameters;
using LoadFamilies;

namespace CC_Plugin
{
    public static class LoadFamTab
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Plugin.dll";
            return dll;
        }
        public static void LoadFamPanel(this UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Load Families");

            PushButtonData b1Data = new PushButtonData(
                "Load MEP Fams",
                "Load MEP Families",
                @dllpath(),
                "CC_Plugin.LoadMEPFamilies");
            b1Data.ToolTip = "Load MEP Related Families.";

            PushButtonData b2Data = new PushButtonData(
                "Load Ceiling Pallets",
                "Load Ceiling Pallets",
                @dllpath(),
                "CC_Plugin.LoadCeilings");
            b2Data.ToolTip = "Load a Pallet of Materials used for ceilings.";

            PushButtonData b3Data = new PushButtonData(
                "Load Symbols",
                "Load Symbols",
                @dllpath(),
                "CC_Plugin.LoadSymbols");
            b3Data.ToolTip = "Load typical symbols into the project.";

            PushButtonData b4Data = new PushButtonData(
                "Add Parameters",
                "Add Parameters",
                @dllpath(),
                "CC_Plugin.AddRevitParameters");
            b4Data.ToolTip = "Add Standard Parameters to the Project";

            PushButton b4 = CPanel.AddItem(b4Data) as PushButton;

            var DBButtons = new List<RibbonItem>();
            DBButtons.AddRange(CPanel.AddStackedItems(b1Data, b2Data, b3Data));

        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class LoadSymbols : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            using (Transaction t = new Transaction(doc, "Load"))
            {
                t.Start();
                doc.LoadSymbols();
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class AddRevitParameters : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            doc.AddParams();
            return Result.Succeeded;
        }
    }
}