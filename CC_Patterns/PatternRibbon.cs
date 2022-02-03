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
            cbox.AddItem(new ComboBoxMemberData("Basket Pattern", "Basket Pattern"));
            cbox.AddItem(new ComboBoxMemberData("Plank Pattern", "Plank Pattern"));
            cbox.AddItem(new ComboBoxMemberData("Custom Pattern", "Custom Pattern"));

            tbox.Width = 250;
            tbox.PromptText = "Width, Height, Grout, Steps";
            tbox.EnterPressed += EnterPressed;
            cbox.CurrentChanged += CurrentChanged;
            return Result.Succeeded;
        }
        private static void CurrentChanged(object sender, ComboBoxCurrentChangedEventArgs args)
        {
            ComboBox cbox = sender as ComboBox;
            var combotype = GetComboData(args.Application);
            switch(combotype)
            {
                default:
                case "Brick Pattern":
                    SetTB(args.Application, "Width, Height, Grout Width, Grout Height, Steps");
                    break;
                case "Herringbone Pattern":
                    SetTB(args.Application, "Width, Height");
                    break;
                case "Basket Pattern":
                    SetTB(args.Application, "Width, Height");
                    break;
                case "Plank Pattern":
                    SetTB(args.Application, "Width, Height, Grout");
                    break;
                case "Drawn Pattern":
                    SetTB(args.Application, "Draw Detail Lines");
                    break;
            }
        }
        private static void EnterPressed(object sender, TextBoxEnterPressedEventArgs args)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Value as string;
            var combotype = GetComboData(args.Application);
            CreatePattern(args.Application.ActiveUIDocument.Document, combotype, text);
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
        private static void SetTB(UIApplication app, string val)
        {
            try
            {
                var panels = app.GetRibbonPanels(TabName);
                var panel = panels.Where(x => x.Name == PanelName).First();
                var items = panel.GetItems();
                var item = items.Where(x => x.ItemType == RibbonItemType.TextBox).First();
                var tb = item as TextBox;
                tb.PromptText = val;
                tb.Value = string.Empty;
            }
            catch {}
        }
        public static void CreatePattern(Document doc, string combotype, string text)
        {
            var numbs = text.Split(',');
            double width = double.TryParse(numbs[0], out double a) ? a : 4;
            double height = numbs.Count() >= 2 ? double.TryParse(numbs[1], out double b) ? b : 2 : 2;
            double groutx = numbs.Count() >= 3 ? double.TryParse(numbs[2], out double c) ? c : 0 : 0;
            double grouty = numbs.Count() >= 4 ? double.TryParse(numbs[3], out double d) ? d : 0 : 0;
            int ratio = numbs.Count() >= 5 ? int.TryParse(numbs[4], out int e) ? e : 2 : 2;
            switch (combotype)
            {
                default:
                case "Brick Pattern":
                    BrickPattern.CreatePattern(doc, width, height, groutx, grouty, ratio);
                    break;
                case "Herringbone Pattern":
                    HerringbonePattern.CreatePattern(doc, width, height);
                    break;
                case "Basket Pattern":
                    BasketPattern.CreatePattern(doc, width, height);
                    break;
            }
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
}
