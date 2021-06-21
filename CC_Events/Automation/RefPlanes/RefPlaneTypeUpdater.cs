using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB.Events;

using CC_Library;
using CC_Library.Predictions;
using CC_Library.Parameters;

using CC_Plugin.Parameters;

namespace CC_Plugin
{
    public class PlaneTypeUpdater : IUpdater
    {
        public static void OnStartup(UIControlledApplication application)
        {
            PlaneTypeUpdater updater = new PlaneTypeUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);

            ElementClassFilter refFilter = new ElementClassFilter(typeof(ReferencePlane));

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), refFilter, Element.GetChangeTypeAny());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), refFilter, Element.GetChangeTypeElementAddition());
        }

        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            PlaneTypeUpdater updater = new PlaneTypeUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        static AddInId appId;
        static UpdaterId updaterId;

        public PlaneTypeUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("c8eda7b3-a1fd-496c-8e98-0438a2f8c9dd"));
        }
        public void Execute(UpdaterData data)
        {
            try
            {
                Document doc = data.GetDocument();
                if (doc.IsFamilyDocument)
                {
                    var modrps = data.GetModifiedElementIds();
                    var addrps = data.GetAddedElementIds();

                    List<ElementId> rps = modrps.ToList();
                    rps.AddRange(addrps.ToList());
                    foreach (ElementId eid in rps)
                    {
                        ReferencePlane rp = doc.GetElement(eid) as ReferencePlane;
                        if (rp.ParametersMap.get_Item("Is Reference").AsInteger() == 14)
                            rp.ParametersMap.get_Item("Is Reference").Set(12);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.OutputError();
            }
        }
        public string GetAdditionalInformation() { return "Collects information about reference planes in a family"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Charlie Crowe Reference Plane Data Collector"; }
    }
}