using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using CC_Library;
using CC_Plugin.TypeNaming;
using CC_Plugin.Details;

namespace CC_Plugin
{
    public class HatchEditor
    {
        public void EditHatch(Document doc)
        {
            if(doc.OwnerFamily.FamilyCategory == Category.GetCategory(doc, BuiltInCategory.OST_DetailComponents))
            {
                var v = doc.ActiveView;
                var lines = new FilteredElementCollector(doc, v.Id).OfCategory(BuiltInCategory.OST_Lines).ToElementIds().ToList();
                List<double[]> points = new List<double[]>();
                for(int i = 0; i < lines.Count(); i++)
                {
                    var line = doc.GetElement(lines[i]) as DetailLine;
                    if (line != null)
                    {
                        var pt = new double[4];
                        pt[0] = line.GeometryCurve.GetEndPoint(0).X;
                        pt[1] = line.GeometryCurve.GetEndPoint(0).Y;
                        pt[2] = line.GeometryCurve.GetEndPoint(1).X;
                        pt[3] = line.GeometryCurve.GetEndPoint(1).Y;
                    }
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
