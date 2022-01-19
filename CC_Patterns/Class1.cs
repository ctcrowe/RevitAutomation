using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using CC_Library;
using CC_Plugin.TypeNaming;
using CC_Plugin.Details;

namespace CC_Patterns
{
    public class CC_PatternRibbon : IExternalApplication
    {
        private const string TabName = "CCrowe";
        private const string PanelName = "Patterns";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            RibbonPanel Panel = uiApp.CreateRibbonPanel(TabName, PanelName);
            TextBoxData tbd = new TextBoxData("Pattern Entry");
            ComboBoxData cbd = new ComboBoxData("Pattern Type");
            var items = Panel.AddStackedItems(cbd, tbd);
            var cbox = items[0] as ComboBox;
            var tbox = items[1] as TextBox;
            
            cbox.AddItem(new ComboBoxMemberData("Brick Pattern", "Brick Pattern"));
            cbox.AddItem(new ComboBoxMemberData("Herringbone Pattern", "Herringbone Pattern"));
            
            return Result.Succeeded;
        }
        private static void EnterPressed(object sender, TextBoxEnterPressedEventArgs args)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Value as string;
            var combotype = args.Application.GetComboData();
            combotype.CreatePattern(text);
        }
        private static string GetComboData(this UIApplication app)
        {
            string val = "";
            try
            {
                var panels = app.GetRibbonPanels(TabName);
                var panel = panels.Where(x => x.Name == PanelName).First();
                var items = panel.GetItems();
                var item = items.Where(x => x.ItemType == RibbonItemType.ComboBox).First();
                var cb = item as ComboBox;
                val = cb.Current.Name;
            }
            catch (Exception e) { e.OutputError(); }
            return val;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
}
