using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using System.Reflection;

namespace CC_ZeroPoint
{
    public class CC_ZeroPointRibbon : IExternalApplication
    {
        private static string dllpath = Assembly.GetExecutingAssembly().Location;
        private const string TabName = "CCrowe";
        private const string PanelName = "Zero Point";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            try { uiApp.CreateRibbonTab(TabName); } catch {};
            RibbonPanel Panel = uiApp.CreateRibbonPanel(TabName, PanelName);
            ComboBoxData cb1 = new ComboBoxData("Height");
            ComboBoxData cb2 = new ComboBoxData("Width");
            PushButtonData b1d = new PushButtonData(
                "Create Detail Border",
                "Create Detail Border",
                @dllpath,
                "CC_ZeroPoint.CreateDetailBorder");
            b1d.ToolTip = "Create a reference border for the active detail view.";
            
            var items = Panel.AddStackedItems(cb1, cb2, b1d);
            var cbox1 = items[0] as ComboBox;
            var cbox2 = items[1] as ComboBox;
            
            cbox1.AddItem(new ComboBoxMemberData("Height - 1", "Height - 1"));
            cbox1.AddItem(new ComboBoxMemberData("Height - 2", "Height - 2"));
            cbox1.AddItem(new ComboBoxMemberData("Height - 3", "Height - 3"));
            cbox1.AddItem(new ComboBoxMemberData("Height - 4", "Height - 4"));
            
            cbox2.AddItem(new ComboBoxMemberData("Width - 1", "Width - 1"));
            cbox2.AddItem(new ComboBoxMemberData("Width - 2", "Width - 2"));
            cbox2.AddItem(new ComboBoxMemberData("Width - 3", "Width - 3"));
            cbox2.AddItem(new ComboBoxMemberData("Width - 4", "Width - 4"));
            cbox2.AddItem(new ComboBoxMemberData("Width - 5", "Width - 5"));
            
            return Result.Succeeded;
        }
        public static int[] GetComboData(UIApplication app)
        {
            int[] val = new int[2];
            try
            {
                var panels = app.GetRibbonPanels(TabName);
                var panel = panels.Where(x => x.Name == PanelName).First();
                var items = panel.GetItems();

                var item1 = items.Where(x => x.ItemType == RibbonItemType.ComboBox).ElementAt(0);
                var item2 = items.Where(x => x.ItemType == RibbonItemType.ComboBox).ElementAt(1);
                var cb1 = item1 as ComboBox;
                var cb2 = item2 as ComboBox;
                val[0] = int.Parse(cb1.Current.Name.Split(' ').Last());
                val[1] = int.Parse(cb2.Current.Name.Split(' ').Last());
            }
            catch (Exception e) { }
            return val;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CreateDetailBorder : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            GetZeroPointBox.CreateBox(commandData.Application);
            return Result.Succeeded;
        }
    }
}
