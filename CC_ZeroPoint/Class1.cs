using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

namespace CC_Patterns
{
    public class CC_ZeroPointRibbon : IExternalApplication
    {
        private const string TabName = "CCrowe";
        private const string PanelName = "Zero Point";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            try { uiApp.CreateRibbonTab(TabName); } catch {};
            RibbonPanel Panel = uiApp.CreateRibbonPanel(TabName, PanelName);
            ComboBoxData cb1 = new ComboBoxData("Height");
            ComboBoxData cb2 = new ComboBoxData("Width");
            PushButtonData b1d = new PushButtonData();
            
            var items = Panel.AddStackedItems(cb1, cb2, b1d);
            var cbox1 = items[0] as ComboBox;
            var cbox2 = items[2] as ComboBox;
            
            cbox1.AddItem(new ComboBoxMemberData("1 - One", "1 - One"));
            cbox1.AddItem(new ComboBoxMemberData("2 - Two", "2 - Two"));
            cbox1.AddItem(new ComboBoxMemberData("3 - Three", "3 - Three"));
            cbox1.AddItem(new ComboBoxMemberData("4 - Four", "4 - Four"));
            
            cbox2.AddItem(new ComboBoxMemberData("1 - One", "1 - One"));
            cbox2.AddItem(new ComboBoxMemberData("2 - Two", "2 - Two"));
            cbox2.AddItem(new ComboBoxMemberData("3 - Three", "3 - Three"));
            cbox2.AddItem(new ComboBoxMemberData("4 - Four", "4 - Four"));
            
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
                
                var item1 = items.Where(x => x.ItemType == RibbonItemType.ComboBox)[0];
                var item1 = items.Where(x => x.ItemType == RibbonItemType.ComboBox)[1];
                var cb = item as ComboBox;
                val = cb.Current.Name;
            }
            catch (Exception e) { }
            return val;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
}
