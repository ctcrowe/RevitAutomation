using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CC_Plugin
{
    public class RefPlaneData
    {
        public static void OnStartup(UIControlledApplication application)
        {
            PlaneTypeUpdater updater = new PlaneTypeUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);

            ElementClassFilter refFilter = new ElementClassFilter(typeof(ReferencePlane));

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), refFilter, Element.GetChangeTypeElementAddition());
        }

        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            PlaneTypeUpdater updater = new PlaneTypeUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
    }
    public class PlaneTypeUpdater : IUpdater
    {
        static AddInId appId;
        static UpdaterId updaterId;

        public PlaneTypeUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("c8eda7b3-a1fd-496c-8e98-0438a2f8c9dd"));
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            if(doc.IsFamilyDocument)
            {
                FilteredElementCollector refcollector = new FilteredElementCollector(doc).OfClass(typeof(ReferencePlane));
                List<ElementId> rplanes = refcollector.ToElementIds().ToList();
                List<string> db = new List<string>();
                foreach (ElementId eid in rplanes)
                {
                    ReferencePlane rp = doc.GetElement(eid) as ReferencePlane;
                    string type = "";
                    if(rp.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).AsValueString() != null)
                        type = rp.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).AsValueString();
                    string direction = rp.Direction.X + " ; " + rp.Direction.Y + " ; " + rp.Direction.Z;
                    string loc = rp.BubbleEnd.X + " ; " + rp.BubbleEnd.Y + " ; " + rp.BubbleEnd.Z;
                    string IsRef = "";
                    if(rp.ParametersMap.get_Item("Is Reference").AsValueString() != null)
                        IsRef = rp.ParametersMap.get_Item("Is Reference").AsValueString();
                    db.Add("\r\n" + type + "\t" + direction + "\t" + loc + "\t" + IsRef);
                }
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string subdir = dir + "\\CC_Data";
                if (!Directory.Exists(subdir))
                    Directory.CreateDirectory(subdir);
                string file = subdir + "\\" + doc.PathName.Split('.').First().Split('\\').Last() + "_data.txt";
                if (File.Exists(file))
                    File.Delete(file);

                File.WriteAllLines(file, db);
            }
        }
        public string GetAdditionalInformation() { return "Collects information about reference planes in a family"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Charlie Crowe Reference Plane Data Collector"; }
    }
}