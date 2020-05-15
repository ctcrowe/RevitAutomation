using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_Library.Datatypes;
using CC_RevitBasics;

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

            foreach (ElementId eid in eids)
            {
                Element ele = doc.GetElement(eid);
                FamilyInstance inst = ele as FamilyInstance;
                if (inst != null)
                {
                    string id = inst.GetElementTypeParam(CCParameter.CC_ID);
                    string name = inst.Symbol.Family.Name;
                    Datatype.Masterformat.CreateInputData(name, inst.GetElementTypeParam(CCParameter.Masterformat));
                    /*
                    var cats = inst.GetCategories();
                    if (cats.Any())
                    {
                        foreach (var c in cats)
                        {
                            //try { Datatype.Subcategory.CreateInputData(name, c); }
                            //catch { TaskDialog.Show("Error", "Failed to create category at Updater"); }
                        }
                    }
                    */
                }
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            CC_Automation updater = new CC_Automation(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementClassFilter instFilter = new ElementClassFilter(typeof(FamilyInstance));
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
