using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using CC_Library;
using CC_Plugin.TypeNaming;

namespace CC_Plugin
{
    public class CCRibbon : IExternalApplication
    {
        public const string tabName = "CCrowe";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            try { uiApp.CreateRibbonTab(tabName); } catch {};
            CCPaintPanel.PaintPanel(uiApp);
            UpdateTab.CreatePanel(uiApp);
            ObjStylesTab.ObjTab(uiApp, tabName);
            LineweightTab.LWTab(uiApp, tabName);

           // try { LineStyleNetworkUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { TypeNamingUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { DocumentSaved.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { PlaneTypeUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            //try { ObjStyleNetworkUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            //LineStyleNetworkUpdater.OnShutdown(uiApp);
            TypeNamingUpdater.OnShutdown(uiApp);
            DocumentSaved.OnShutdown(uiApp);
            PlaneTypeUpdater.OnShutdown(uiApp);
            //ObjStyleNetworkUpdater.OnShutdown(uiApp);
            return Result.Succeeded;
        }
    }
}
