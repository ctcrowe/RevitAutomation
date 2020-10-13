using System.Reflection;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public class CCRibbon
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

        public static void OnStartup(UIControlledApplication uiApp)
        {
            uiApp.CreateRibbonTab(tabName);
            
            QCTab.QCPanel(uiApp, tabName);
            PrintPanel(uiApp);
            MFPanel.MFTab(uiApp, tabName);
            uiApp.LoadFamPanel(tabName);

            try
            {
                UpdateOccupantLoads.OnStartup(uiApp);
                ViewChanged.OnStartup(uiApp);
                DocSynching.OnStartup(uiApp);
                RefPlaneMaker.OnStartup(uiApp);
                CC_Automation.OnStartup(uiApp);
                UpdateLoadFactor.OnStartup(uiApp);
                UpdateStudSize.OnStartup(uiApp);
                FamLoadedEvent.OnStartup(uiApp);
                DocSavingAs.OnStartup(uiApp);
                DocClosing.OnStartup(uiApp);
            }
            catch { }
        }
        public static void OnShutdown(UIControlledApplication uiApp)
        {
            UpdateOccupantLoads.OnShutdown(uiApp);
            ViewChanged.OnShutdown(uiApp);
            RefPlaneMaker.OnShutdown(uiApp);
            CC_Automation.OnShutdown(uiApp);
            UpdateLoadFactor.OnShutdown(uiApp);
            UpdateStudSize.OnShutdown(uiApp);
            DocSynching.OnShutdown(uiApp);
            FamLoadedEvent.OnShutdown(uiApp);
            DocSavingAs.OnShutdown(uiApp);
            DocClosing.OnShutdown(uiApp);
        }
    }
}
