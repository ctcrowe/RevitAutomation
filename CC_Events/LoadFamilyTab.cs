using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

using MEPEquipment;
using CC_CeilingTypes;
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
            b1Data.ToolTip = "Load a Pallet of Materials used for ceilings.";

            PushButtonData b3Data = new PushButtonData(
                "Load Symbols",
                "Load Symbols",
                @dllpath(),
                "CC_Plugin.LoadSymbols");
            b1Data.ToolTip = "Load typical symbols into the project.";

            var DBButtons = new List<RibbonItem>();
            DBButtons.AddRange(CPanel.AddStackedItems(b1Data, b2Data, b3Data));

        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class LoadMEPFamilies : IExternalCommand
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
                doc.LoadMEPEquipment();
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class LoadCeilings : IExternalCommand
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
                doc.LoadCeilings();
                t.Commit();
            }
            return Result.Succeeded;
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
}
