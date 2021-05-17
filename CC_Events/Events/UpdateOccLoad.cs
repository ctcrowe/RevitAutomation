using System.Collections.Generic;
using System.Linq;
using System;
using CC_Library.Parameters;
using CC_Plugin.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using CC_Library.Predictions;

namespace CC_Plugin
{
    /*
    TODO: On element placed, check if its location is inside of a room.
        If it is not inside of a room place a room in that location.
        If that room is not enclosed, delete the room added.
        If it is enclosed, exit process.
    */

    internal class UpdateOccLoad : IUpdater
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            var Rooms = data.GetModifiedElementIds().ToList();
            foreach(var room in Rooms)
            {
                Element e = doc.GetElement(room);
                Room r = e as Room;
                if (r != null)
                {
                    double lf = double.Parse(e.GetElementParam(RoomParams.OccupantLoadFactor).Split(' ').First());
                    double Area = r.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
                    int load = 0;
                    if (lf > 0)
                        load = (int)Math.Ceiling(Area / lf);
                    r.SetIntParam(RoomParams.OccupantLoad, load.ToString());
                }
            }
        }
        public static void ProjectStartup(AddInId id, Document doc)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Preupdater Registration"))
            {
                tg.Start();
                if (!doc.IsFamilyDocument)
                {
                    using (Transaction t = new Transaction(doc, "Add Parameters"))
                    {
                        t.Start();
                        doc.AddParam(RoomParams.OccupantLoad);
                        doc.AddParam(RoomParams.OccupantLoadFactor);
                        t.Commit();
                    }
                    Room r;
                    using (Transaction t = new Transaction(doc, "Add Room"))
                    {
                        t.Start();
                        r = doc.Create.NewRoom(doc.Phases.get_Item(0));
                        t.Commit();
                    }
                    Element room = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).FirstOrDefault();
                    Parameter p = room.get_Parameter(RoomParams.OccupantLoadFactor.Guid);

                    UpdateOccLoad updater = new UpdateOccLoad(id);
                    UpdaterRegistry.RegisterUpdater(updater, doc, true);
                    ElementCategoryFilter rooms = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);
                    ElementId AreaId = new ElementId(BuiltInParameter.ROOM_AREA);
                    UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), rooms, Element.GetChangeTypeParameter(AreaId));
                    UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), rooms, Element.GetChangeTypeParameter(p.Id));

                    using (Transaction t = new Transaction(doc, "Add Room"))
                    {
                        t.Start();
                        doc.Delete(r.Id);
                        t.Commit();
                    }
                }
                tg.Commit();
            }
        }
        public static void ProjectShutdown(AddInId id, Document doc)
        {
            UpdateOccLoad updater = new UpdateOccLoad(id);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId(), doc);
        }
        public UpdateOccLoad(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("27172da1-9646-4bd5-8d44-a4c89f007010"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Updates Occupant Loads When Area or Load Factor Change"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update Load Factor"; }
    }
}
