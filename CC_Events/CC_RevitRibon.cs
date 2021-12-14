﻿using System.Reflection;
using System;
using Autodesk.Revit.UI;
using CC_Library;
using CC_Plugin.TypeNaming;
using CC_Plugin.Details;

namespace CC_Plugin
{
    public class HatchEditor
    {
        public void EditHatch(Document doc)
        {
            if(doc.OwnerFamily.FamilyCategory == FamilyCategories.DetailItem)
            {
                Document doc = v.Document;
                var lines = new FilteredElementCollector(doc, v.Id).OfCategory(BuiltInCategory.OST_Lines).ToElements().ToList();
                foreach(var line in lines)
                {
                }
            }
            else
            {
            }
        }
    }
}
/* hatch editor
// if(doc.IsFamilyDocument)
// {
//      if(doc.OwnerFamily.FamilyCategory == Detail Item)
//      {
            Get Line Start Point
            Get Line End Point
            Add to List<string>Lines();
            //Generate Title
            //Write All Lines to File
            //Profit
//      }
// }
*/

namespace CC_Plugin
{
    public class CCRibbon : IExternalApplication
    {
        public const string tabName = "CCrowe";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            uiApp.CreateRibbonTab(tabName);
            CCPaintPanel.PaintPanel(uiApp);
            UpdateTab.CreatePanel(uiApp);
            FamilyReorganize.Run();

            //DetailPanel.DetailTab(uiApp, tabName);
            //ObjStylesTab.ObjTab(uiApp, tabName);
            //QCTab.QCPanel(uiApp, tabName);
            // MFPanel.MFTab(uiApp, tabName);
            //AnalysisTab.AnalysisPanel(uiApp, tabName);
            //SchduleTab.SchedulePanel(uiApp, tabName);
            //uiApp.LoadFamPanel(tabName);

            //try { LineStyleUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { LineStyleNetworkUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { MFTypeNameChange.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { TypeNamingUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { DocumentSaved.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { PlaneTypeUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            //try { ObjStyleUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            try { ObjStyleNetworkUpdater.OnStartup(uiApp); } catch (Exception e) { e.OutputError(); }
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            //LineStyleUpdater.OnShutdown(uiApp);
            LineStyleNetworkUpdater.OnShutdown(uiApp);
            MFTypeNameChange.OnShutdown(uiApp);
            TypeNamingUpdater.OnShutdown(uiApp);
            DocumentSaved.OnShutdown(uiApp);
            PlaneTypeUpdater.OnShutdown(uiApp);
            //ObjStyleUpdater.OnShutdown(uiApp);
            ObjStyleNetworkUpdater.OnShutdown(uiApp);
            return Result.Succeeded;
        }
    }
}
