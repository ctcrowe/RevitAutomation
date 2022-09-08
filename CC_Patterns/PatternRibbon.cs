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
            TextBoxData tbd1 = new TextBoxData("Width");
            TextBoxData tbd2 = new TextBoxData("Height");
            TextBoxData tbd3 = new TextBoxData("Grout Width");
            TextBoxData tbd4 = new TextBoxData("Grout Height");
            TextBoxData tbd5 = new TextBoxData("Spacing");
            ComboBoxData cbd = new ComboBoxData("Pattern Type");
            var items1 = Panel.AddStackedItems(cbd, tbd1, tbd2);
            var items2 = Panel.AddStackedItems(tbd3, tbd4, tbd5);
            var cbox = items1[0] as ComboBox;
            var WidthTB = items1[1] as TextBox;
            var HeightTB = items1[2] as TextBox;
            var GroutWidthTB = items2[0] as TextBox;
            var GroutHeightTB = items2[1] as TextBox;
            var SpacingTB = items2[2] as TextBox;
            
            var Brick = new ComboBoxMemberData("Brick Pattern", "Brick Pattern");
            Brick.ToolTip = "Used for creating brick and tiling patterns of all types including a direct stack.";
            cbox.AddItem(Brick);
            
            var Herringbone = new ComboBoxMemberData("Herringbone Pattern", "Herringbone Pattern");
            Herringbone.ToolTip = "Used to create Herringbone Pattern Files. Format is Width (decimal inches), Height (decimal inches). Grout spacing is defined automatically based on pattern.";
            cbox.AddItem(Herringbone);
            
            var Basket = new ComboBoxMemberData("Basket Pattern", "Basket Pattern");
            Basket.ToolTip = "Creates a Basket Tile Pattern File. Format is Width (decimal inches), Height (decimal inches). Grout spacing is automatically determined.";
            cbox.AddItem(Basket);
            
            var Plank = new ComboBoxMemberData("Plank Pattern", "Plank Pattern");
            Plank.ToolTip = "Creates a linear pattern of a given length with randomly dispersed endpoints, creating a wood flooring style effect. Format is Width (Decimal Inches),Height (Decimal Inches), Grout (Decimal Inches)";
            cbox.AddItem(Plank);
            
            cbox.AddItem(new ComboBoxMemberData("Custom Pattern", "Custom Pattern"));

            WidthTB.PromptText = "Width";
            HeightTB.PromptText = "Height";
            GroutWidthTB.PromptText = "Grout Width";
            GroutHeightTB.PromptText = "Grout Height";
            SpacingTB.PromptText = "Spacing";
            
            //tbox.Width = 250;
            WidthTB.Width = 200;
            HeightTB.Width = 200;
            GroutWidthTB.Width = 200;
            GroutHeightTB.Width = 200;
            SpacingTB.Width = 200;
            
            WidthTB.EnterPressed += EnterPressed;
            HeightTB.EnterPressed += EnterPressed;
            GroutWidthTB.EnterPressed += EnterPressed;
            GroutHeightTB.EnterPressed += EnterPressed;
            SpacingTB.EnterPressed += EnterPressed;
            
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
                    SetTB(args.Application, "Width", "Height", "Grout Width", "Grout Height", "Steps");
                    break;
                case "Herringbone Pattern":
                    SetTB(args.Application, "Width", "Height", "null", "null", "null");
                    break;
                case "Basket Pattern":
                    SetTB(args.Application, "Width", "Height", "null", "null", "null");
                    break;
                case "Plank Pattern":
                    SetTB(args.Application, "Width", "Height", "Grout", "null", "null");
                    break;
                case "Drawn Pattern":
                    SetTB(args.Application, "null", "null", "null", "null", "null");
                    break;
            }
        }
        private static void EnterPressed(object sender, TextBoxEnterPressedEventArgs args)
        {
            TextBox textBox = sender as TextBox;
            string text = CreateTextData(args.Application);
            var combotype = GetComboData(args.Application);
            CreatePattern(args.Application.ActiveUIDocument.Document, combotype, text);
        }
        private static string CreateTextData(UIApplication app)
        {
            string val = "";
            try
            {
                var panels = app.GetRibbonPanels(TabName);
                var panel = panels.Where(x => x.Name == PanelName).First();
                var items = panel.GetItems();
                
                var WidthBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Width").First() as TextBox;
                var HeightBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Height").First() as TextBox;
                var GWidthBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Grout Width").First() as TextBox;
                var GHeightBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Grout Height").First() as TextBox;
                var SpacingBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Spacing").First() as TextBox;
                
                string w = WidthBox.Value as string;
                string h = HeightBox.Value as string;
                string gw = GWidthBox.Value as string;
                string gh = GHeightBox.Value as string;
                string s = SpacingBox.Value as string;
                
                val = w + "," + h + "," + gw + "," + gh + "," + s;
            }
            catch (Exception e) { }
            return val;
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
        private static void SetTB(UIApplication app, string v1, string v2, string v3, string v4, string v5)
        {
            try
            {
                var panels = app.GetRibbonPanels(TabName);
                var panel = panels.Where(x => x.Name == PanelName).First();
                var items = panel.GetItems();
                
                var WidthBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Width").First() as TextBox;
                var HeightBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Height").First() as TextBox;
                var GWidthBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Grout Width").First() as TextBox;
                var GHeightBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Grout Height").First() as TextBox;
                var SpacingBox = items.Where(x => x.ItemType == RibbonItemType.TextBox).Where(y => y.Name == "Spacing").First() as TextBox;
                
                
                WidthBox.PromptText = v1;
                WidthBox.Value = string.Empty;
                WidthBox.Enabled = v1 != "null";
                HeightBox.PromptText = v2;
                HeightBox.Value = string.Empty;
                HeightBox.Enabled = v2 != "null";
                GWidthBox.PromptText = v3;
                GWidthBox.Value = string.Empty;
                GWidthBox.Enabled = v3 != "null";
                GHeightBox.PromptText = v4;
                GHeightBox.Value = string.Empty;
                GHeightBox.Enabled = v4 != "null";
                SpacingBox.PromptText = v5;
                SpacingBox.Value = string.Empty;
                SpacingBox.Enabled = v5 != "null";
            }
            catch (Exception e) { }
        }
        public static bool CreatePattern(Document doc, string combotype, string text)
        {
            var numbs = text.Split(',');
            if(!double.TryParse(numbs[0], out double a)) { return false; }
            if(numbs.Count() < 2) { return false; }
            if(!double.TryParse(numbs[1], out double b)) { return false; }
            
            double width = double.TryParse(numbs[0], out double f) ? f : 4;
            double height = numbs.Count() >= 2 ? double.TryParse(numbs[1], out double g) ? g : 2 : 2;
            
            double groutx = numbs.Count() >= 3 ? double.TryParse(numbs[2], out double c) ? c : 0 : 0;
            double grouty = numbs.Count() >= 4 ? double.TryParse(numbs[3], out double d) ? d : double.TryParse(numbs[2], out double h) ? h : 0 : 0;
            
            int ratio = numbs.Count() >= 5 ? int.TryParse(numbs[4], out int e) ? e : 1 : 1;
            
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
                case "Plank Pattern":
                    PlankPattern.CreatePattern(doc, width, height, groutx);
                    break;
            }
            return true;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
}
