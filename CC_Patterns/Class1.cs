using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

namespace CC_Patterns
{
    public class CC_PatternRibbon : IExternalApplication
    {
        private const string TabName = "CCrowe";
        private const string PanelName = "Patterns";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            try { uiApp.CreateRibbonTab(TabName); } catch {};
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
            var combotype = GetComboData(args.Application);
            CreatePattern(combotype, text);
        }
        private static string GetComboData(UIApplication app)
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
            catch (Exception e) { }
            return val;
        }
        public static void CreatePattern(string combotype, string text)
        {
            var numbs = text.Split(',');
            double width = double.TryParse(numbs[0], out double a) ? a : 4;
            double height = numbs.Count() >= 2 ? double.TryParse(numbs[1], out double b) ? b : 2 : 2;
            double grout = numbs.Count() >= 3 ? double.TryParse(numbs[2], out double c) ? c : 0 : 0;
            int ratio = numbs.Count() >= 4 ? int.TryParse(numbs[3], out int d) ? d : 2 : 2;
            switch (combotype)
            {
                default:
                    case "Brick Pattern":
                        BrickPattern.CreatePattern(width, height, grout, ratio);
                        break;
                    case "Herringbone Pattern":
                        HerringbonePattern.CreatePattern(width, height);
                        break;
            }
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
}
