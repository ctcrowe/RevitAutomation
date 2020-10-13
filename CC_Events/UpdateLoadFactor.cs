using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using CC_Library.Datatypes;
using CC_Library.Predictions;
using CC_RevitBasics;

namespace CC_Plugin
{
    /*
    TODO: On element placed, check if its location is inside of a room.
        If it is not inside of a room place a room in that location.
        If that room is not enclosed, delete the room added.
        If it is enclosed, exit process.
    */

    internal class UpdateLoadFactor : IUpdater
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public void Execute(UpdaterData data)
        {
            List<ElementId> eids = data.GetModifiedElementIds().ToList();
            Document doc = data.GetDocument();

            foreach (ElementId eid in eids)
            {
                Element ele = doc.GetElement(eid);
                Room r = ele as Room;
                if (r != null)
                {
                    /*
                    Parameter p = r.get_Parameter(BuiltInParameter.ROOM_NAME);
                    string s = p.AsString();
                    //var OLF = s.PredictOLF();
                    double lf = double.Parse(OLF);
                    double Area = r.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
                    int load = 0;
                    if(lf > 0)
                        load = (int)Math.Ceiling(Area / lf);
                    TaskDialog.Show("Load Factor Change", "The New Load Factor for " + s + " is " + OLF + "\r\n"
                        + "The new Occupany Load is " + load);
                    ele.SetElementParam(CCParameter.OccupantLoadFactor, OLF);
                    ele.SetElementParam(CCParameter.OccupantLoad, load.ToString());
                    */
                }
                else
                    TaskDialog.Show("Error", "The room was null");
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            UpdateLoadFactor updater = new UpdateLoadFactor(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementCategoryFilter rooms = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);
            ElementId NameID = new ElementId(BuiltInParameter.ROOM_NAME);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), rooms, Element.GetChangeTypeParameter(NameID));
        }
        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            UpdateLoadFactor updater = new UpdateLoadFactor(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        public UpdateLoadFactor(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("e3ac1db9-b42b-42fd-b824-0a9ea3ce2d50"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Standard Automation Loop Used By Charlie Crowe"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update Load Factor"; }
    }
}