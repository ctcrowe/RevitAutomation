using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;

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
                    string time = ProjectTime.Get(id);
                    if (time != null)
                    { try { dataset.Add("ProjectTime", time); } catch { } }
                    try { dataset.Add("RmName", inst.Room.Name); } catch { }
                    try { dataset.Add("RmNumber", inst.Room.Number.ToString()); } catch { }
                    try { dataset.Add("ViewType", doc.ActiveView.GetType().Name); } catch { }
                    
                    Datapoint.Create(dataset);
                }
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            RoomNameCollector updater = new RoomNameCollector(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementId RMNameID = new ElementId(BuiltInParameter.ROOM_NAME);
            ElementClassFilter roomFilter = new ElementClassFilter(typeof(Room));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), refFilter, Element.GetChangeTypeParameter(RMNameID));
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
