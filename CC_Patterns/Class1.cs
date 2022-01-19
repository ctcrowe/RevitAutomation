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
        public const string tabName = "CCrowe";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            uiApp.CreateRibbonTab(tabName);
            CCPaintPanel.PaintPanel(uiApp);
            UpdateTab.CreatePanel(uiApp);
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }
    }
}
