using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace CC_Events
{
    internal class Cleanup
    {
        public static void PurgeGarbage(Document currentDoc)
        {
            List<ElementId> MarkersToDelete = new List<ElementId>();
            List<ElementId> TemplatesToDelete = new List<ElementId>();
            List<ElementId> FiltersToDelete = new List<ElementId>();

            FilteredElementCollector viewCollector = new FilteredElementCollector(currentDoc);

            List<ElementId> Views = viewCollector.OfCategory(BuiltInCategory.OST_Views).ToElementIds().ToList();
            List<Element> Markers = viewCollector.OfClass(typeof(ElevationMarker)).ToList();

            FilteredElementCollector filtCollector = new FilteredElementCollector(currentDoc);
            List<ElementId> filters = filtCollector.OfClass(typeof(FilterElement)).ToElementIds().ToList();

            for (int i = 0; i < Markers.Count(); i++)
            {
                ElevationMarker marker = Markers[i] as ElevationMarker;
                if(marker.CurrentViewCount == 0)
                {
                    MarkersToDelete.Add(marker.Id);
                }
            }
            
            List<ElementId> templates = new List<ElementId>();
            List<ElementId> usedtemplates = new List<ElementId>();
            List<ElementId> usedfilters = new List<ElementId>();

            for(int i = 0; i < Views.Count(); i++)
            {
                View view = currentDoc.GetElement(Views[i]) as View;
                if(view.IsTemplate)
                {
                    templates.Add(Views[i]);
                }
                else
                {
                    usedtemplates.Add(view.ViewTemplateId);
                }
            }
            for(int i = 0; i < templates.Count(); i++)
            {
                if(!usedtemplates.Contains(templates[i]))
                { TemplatesToDelete.Add(templates[i]); }
            }

            for(int i = 0; i < Views.Count(); i++)
            {
                if(!TemplatesToDelete.Contains(Views[i]))
                {
                    View v = currentDoc.GetElement(Views[i]) as View;
                    List<ElementId> filterlist = v.GetFilters().ToList();
                    for(int j = 0; j < filterlist.Count(); j++)
                    {
                        if(!usedfilters.Contains(filterlist[j]))
                        {
                            usedfilters.Add(filterlist[j]);
                        }
                    }
                }
            }

            for(int i = 0; i < filters.Count(); i++)
            {
                if(!usedfilters.Contains(filters[i]))
                {
                    FiltersToDelete.Add(filters[i]);
                }
            }

            using (Transaction trans = new Transaction(currentDoc, "Purge Elements"))
            {
                trans.Start();
                foreach(ElementId MID in MarkersToDelete)
                { currentDoc.Delete(MID); }
                foreach(ElementId TID in TemplatesToDelete)
                { currentDoc.Delete(TID); }
                foreach(ElementId FID in FiltersToDelete)
                { currentDoc.Delete(FID); }
                trans.Commit();
            }

            TaskDialog.Show("Document Cleanup", "Deleted\r\n" +
                FiltersToDelete.Count() + " Filters\r\n" +
                TemplatesToDelete.Count() + " View Templates\r\n" +
                MarkersToDelete.Count() + "Unused Elevation Markers");

        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ProjectCleanup : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document currentDoc = uiapp.ActiveUIDocument.Document;

            Cleanup.PurgeGarbage(currentDoc);
            return Result.Succeeded;
        }
    }
}
