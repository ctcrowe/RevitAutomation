using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Parameters;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
/*
TODO: On element placed, check if its location is inside of a room.
    If it is not inside of a room place a room in that location.
    If that room is not enclosed, delete the room added.
    If it is enclosed, exit process.
*/
    internal class CC_Automation : IUpdater
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public void Execute(UpdaterData data)
        {
            List<ElementId> eids = data.GetAddedElementIds().ToList();
            Document doc = data.GetDocument();
           

            if (doc.IsFamilyDocument)
            {
                try { FamType.Setup(doc); } catch { }
                try { AddRevitParams.AddFamilyParam(new IDParam(), doc); } catch { }
            }

            IDParam id = new IDParam();
            id.GetIDParam(doc);
            if (string.IsNullOrEmpty(id.Value))
                id.SetIDParam(doc);

            foreach (ElementId eid in eids)
            {
                FamilyInstance inst = doc.GetElement(eid) as FamilyInstance;
                IDParam eleid = new IDParam();
                MasterformatParam mp = new MasterformatParam();
                if (inst != null)
                {
                    Datapoint.Create(doc.GetElement(eid).GetCategories(), directory + "\\CC_CatData");
                    Dictionary<string, string> dataset = new Dictionary<string, string>();
                    try { dataset.Add("Name", inst.Symbol.Family.Name); } catch { }
                    try { dataset.Add("EleID", eleid.GetEleParam(inst)); } catch { }
                    try { dataset.Add("MFSection", mp.GetEleParam(inst)); }  catch { }
                    try { dataset.Add("PrevID", Datapoint.GetPreviousElement(directory + "\\CC_XMLData")); } catch { }
                    try { dataset.Add("PrjID", id.Value); } catch { }
                    try { dataset.Add("PlaceTime", DateTime.Now.ToString("yyyyMMddhhmmss")); } catch { }
                    try { dataset.Add("View", doc.ActiveView.Name); } catch { }
                    try { dataset.Add("Category", inst.Category.Name); } catch { }
                    try { dataset.Add("Delivery", "Automated"); } catch { }
                    try
                    {
                        dataset.Add("EleCount", new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).ToList().Count().ToString());
                    }
                    catch { }

                    string time = ProjectTime.Get(id.Value);
                    if (time != null)
                    { try { dataset.Add("ProjectTime", time); } catch { } }

                    if (!doc.IsFamilyDocument)
                    {
                        try { dataset.Add("RmName", inst.Room.Name); } catch { }
                        try { dataset.Add("RmNumber", inst.Room.Number.ToString()); } catch { }
                        try { dataset.Add("ViewType", doc.ActiveView.GetType().Name); } catch { }
                    }
                    Datapoint.Create(dataset, directory + "\\CC_XMLData");
                }
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            CC_Automation updater = new CC_Automation(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementClassFilter refFilter = new ElementClassFilter(typeof(ReferencePlane));
            ElementClassFilter instFilter = new ElementClassFilter(typeof(FamilyInstance));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), refFilter, Element.GetChangeTypeElementAddition());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), instFilter, Element.GetChangeTypeElementAddition());
        }
        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            CC_Automation updater = new CC_Automation(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        public CC_Automation(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("25be5487-c0fe-4188-b988-889f41397529"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Standard Automation Loop Used By Charlie Crowe"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "CC Automation Loop"; }
    }
}
