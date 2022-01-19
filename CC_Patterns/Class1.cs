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
            items[1].AddItem(new ComboBoxMemberData("Brick Pattern", "Brick Pattern"));
            items[1].AddItem(new ComboBoxMemberData("Herringbone Pattern", "Herringbone Pattern"));
            
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
}
