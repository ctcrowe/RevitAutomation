using System.Reflection;
using System;
using Autodesk.Revit.UI;
using CC_Library;
using CC_Plugin.TypeNaming;
using CC_Plugin.Details;

namespace CC_Plugin
{
    public class CCRibbon : IExternalApplication
    {
        public const string tabName = "CCrowe";
        private static string dllpath = Assembly.GetExecutingAssembly().Location;
        private static void PrintPanel(UIControlledApplication uiApp)
        {
            RibbonPanel PSPanel = uiApp.CreateRibbonPanel(tabName, "Print Sets");

            PushButtonData RevPrintdata = new PushButtonData(
            "Active Revision Sets",
            "Active \r\n Revision \r\n Print Sets",
            @dllpath,
            "CCRevitTools.PrintSets");

            PushButton pbrpd = PSPanel.AddItem(RevPrintdata) as PushButton;
            pbrpd.ToolTip = "Create sheet sets for printing for each active revision";
        }

        public Result OnStartup(UIControlledApplication uiApp)
        {
            uiApp.CreateRibbonTab(tabName);
            //https://spiderinnet.typepad.com/blog/2016/09/revit-net-api-get-all-line-styles.html
            //Used to get linestyles for linestyle updating.
            
            CCPaintPanel.PaintPanel(uiApp);

            //DetailPanel.DetailTab(uiApp, tabName);
            //ObjStylesTab.ObjTab(uiApp, tabName);
            //QCTab.QCPanel(uiApp, tabName);
            //PrintPanel(uiApp);
            // MFPanel.MFTab(uiApp, tabName);
            //AnalysisTab.AnalysisPanel(uiApp, tabName);
            //SchduleTab.SchedulePanel(uiApp, tabName);
            //uiApp.LoadFamPanel(tabName);

            try { LineStyleUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try
            {
                SetMF.OnStartup(uiApp);
                TypeNamingUpdater.OnStartup(uiApp);
                DocumentSaved.OnStartup(uiApp);
                PlaneTypeUpdater.OnStartup(uiApp);
                ObjStyleUpdater.OnStartup(uiApp);
                ObjStyleNetworkUpdater.OnStartup(uiApp);
            }
            catch { TaskDialog.Show("Setup Failed", "Setup Failed"); }
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            LineStyleUpdater.OnShutdown(uiApp);
            SetMF.OnShutdown(uiApp);
            TypeNamingUpdater.OnShutdown(uiApp);
            DocumentSaved.OnShutdown(uiApp);
            PlaneTypeUpdater.OnShutdown(uiApp);
            ObjStyleUpdater.OnShutdown(uiApp);
            ObjStyleNetworkUpdater.OnShutdown(uiApp);
            return Result.Succeeded;
        }
    }
}
