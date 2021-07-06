using System.Reflection;
using Autodesk.Revit.UI;
using CC_Plugin.TypeNaming;
using CC_Plugin.Details;

namespace CC_Plugin
{
    internal class CCPaintPanel
    {
        public static string dllpath = Assembly.GetExecutingAssembly().Location;
        public static void PaintPanel(UIControlledApplication uiApp)
        {
            RibbonPanel Panel = uiApp.CreateRibbonPanel(CCRibbon.tabName, "Paint");
        }
        public static void PaintByMaterial(GenericForm gf)
        {
            {
   Face geoFace = null;

   using (Transaction transaction = new Transaction(document, "Painting"))
   {
      transaction.Start();

      // create a new type and material parameter in the document.
      FamilyManager famMgr = document.FamilyManager;
      if (famMgr.Types.Size == 0)
      {
         FamilyType createdType = famMgr.NewType("New_type");
         famMgr.CurrentType = createdType;
      }

      FamilyParameter materialParam =
         famMgr.AddParameter("Material_Para", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, true);

      famMgr.Set(materialParam, material.Id);

      // Before acquiring the geometry, make sure the detail level is set to 'Fine'
      Options geoOptions = new Options();
      geoOptions.DetailLevel = ViewDetailLevel.Fine;

      // Find the first geometry face of the given extrusion object
      GeometryElement geoElem = extrusion.get_Geometry(geoOptions);
      IEnumerator<GeometryObject> geoObjectItor = geoElem.GetEnumerator();
      while (geoObjectItor.MoveNext())
      {
         // need to find a solid first
         Solid theSolid = geoObjectItor.Current as Solid;
         if (null != theSolid)
         {
            foreach (Face face in theSolid.Faces)
            {
               geoFace = face;
               break;
            }
         }
      }

      if (null == geoFace)
      {
         TaskDialog.Show("Failure", "Could not find a face to paint.");
         transaction.RollBack();
         return;
      }

      // Paint a material family parameter to the extrusion face.
      document.Paint(extrusion.Id, geoFace, materialParam);
      transaction.Commit();
   }

   // For illustration purposes only, check if the painted material indeed got applied

   bool isPainted = document.IsPainted(extrusion.Id, geoFace);
   if (isPainted)
   {
      ElementId paintedMatId = document.GetPaintedMaterial(extrusion.Id, geoFace);
      if (paintedMatId == material.Id)
      {
         TaskDialog.Show("Painting material", "Face painted successfully.");
      }
   }
}
        }
    }
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
            //https://www.revitapidocs.com/2015/f59f8872-e8d7-5d00-0e8c-44a36a843861.htm
            //create a paint all surfaces tool.
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

            try
            {
                SetMF.OnStartup(uiApp);
                TypeNamingUpdater.OnStartup(uiApp);
                DocumentSaved.OnStartup(uiApp);
                PlaneTypeUpdater.OnStartup(uiApp);
                ObjStyleUpdater.OnStartup(uiApp);
                ObjStyleNetworkUpdater.OnStartup(uiApp);

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
            PlaneTypeUpdater.OnShutdown(uiApp);
            ObjStyleUpdater.OnShutdown(uiApp);
            ObjStyleNetworkUpdater.OnShutdown(uiApp);

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
