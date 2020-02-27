using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{

    internal class RoomNameCollector : IUpdater
    {
        public void Execute(UpdaterData data)
        {
            List<ElementId> eids = data.GetModifiedElementIds().ToList();
            Document doc = data.GetDocument();

            foreach (ElementId eid in eids)
            {
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            RoomNameCollector updater = new RoomNameCollector(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementId RMNameID = new ElementId(BuiltInParameter.ROOM_NAME);
            ElementId RMAreaID = new ElementId(BuiltInParameter.ROOM_AREA);
            ElementClassFilter roomFilter = new ElementClassFilter(typeof(Room));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), roomFilter, Element.GetChangeTypeParameter(RMNameID));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), roomFilter, Element.GetChangeTypeParameter(RMAreaID));
        }
        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            RoomNameCollector updater = new RoomNameCollector(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        public RoomNameCollector(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("18513fac-f280-444c-adcb-65644becd142"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Collects name data from rooms for use in predictive algorithms Charlie Crowe"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "CC Room Collector"; }
    }
}
