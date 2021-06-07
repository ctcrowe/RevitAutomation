﻿using System.Reflection;
using Autodesk.Revit.UI;
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
            //uiApp.CreateRibbonTab(tabName);

            //DetailPanel.DetailTab(uiApp, tabName);
            //ObjStylesTab.ObjTab(uiApp, tabName);
            //QCTab.QCPanel(uiApp, tabName);
            //PrintPanel(uiApp);
            // MFPanel.MFTab(uiApp, tabName);
            //AnalysisTab.AnalysisPanel(uiApp, tabName);
            //SchduleTab.SchedulePanel(uiApp, tabName);
            //uiApp.LoadFamPanel(tabName);

            try
            {
                SetMF.OnStartup(uiApp);
                TypeNamingUpdater.OnStartup(uiApp);
                DocumentSaved.OnStartup(uiApp);
                #region IUpdaters
                //MaterialLibrary.OnStartup(uiApp);
                #endregion

                #region Doc Synch Events
                //CollectProjectData.OnStartup(uiApp);
                #endregion

                //RegUpdaters.OnStartup(uiApp);
                //ViewChanged.OnStartup(uiApp);
                //RefPlaneMaker.OnStartup(uiApp);
                //UpdateLoadFactor.OnStartup(uiApp);
                //UpdateStudSize.OnStartup(uiApp);
                //FamLoadedEvent.OnStartup(uiApp);
                //DocSavingAs.OnStartup(uiApp);
                //DocClosing.OnStartup(uiApp);
                //AddParameterEvents.OnStartup(uiApp);
            }
            catch { TaskDialog.Show("Setup Failed", "Setup Failed"); }
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            SetMF.OnShutdown(uiApp);
            TypeNamingUpdater.OnShutdown(uiApp);
            DocumentSaved.OnShutdown(uiApp);
            #region IUpdaters
            //MaterialLibrary.OnShutdown(uiApp);
            #endregion

            #region Doc Synch Events
            //CollectProjectData.OnShutdown(uiApp);
            #endregion

            //RegUpdaters.OnShutdown(uiApp);
            //ViewChanged.OnShutdown(uiApp);
            //RefPlaneMaker.OnShutdown(uiApp);
            //UpdateLoadFactor.OnShutdown(uiApp);
            //UpdateStudSize.OnShutdown(uiApp);
            //FamLoadedEvent.OnShutdown(uiApp);
            //DocSavingAs.OnShutdown(uiApp);
            //DocClosing.OnShutdown(uiApp);
            //AddParameterEvents.OnShutdown(uiApp);
            return Result.Succeeded;
        }
    }
}