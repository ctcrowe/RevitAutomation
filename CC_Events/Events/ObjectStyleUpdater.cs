using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;

using CC_Library;
using CC_Library.Predictions;
using CC_Library.Parameters;

using CC_Plugin.Parameters;

namespace CC_Plugin
{
    internal class ObjStyleUpdater : IUpdater
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            RegisterUpdater(app.ActiveAddInId);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            ObjStyleUpdater updater = new ObjStyleUpdater(app.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }
        public static void RegisterUpdater(AddInId id)
        {
            ObjStyleUpdater updater = new ObjStyleUpdater(id);
            UpdaterRegistry.RegisterUpdater(updater, true);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(GenericForm)),
                Element.GetChangeTypeAny());
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            if (doc.IsFamilyDocument)
            {
                try
                {
                    //Get document Name
                    string name = doc.OwnerFamily.Name;
                    //Get form location info (Varies by type?)
                    foreach (ElementId e in data.GetModifiedElementIds())
                    {
                        Element ele = doc.GetElement(e);
                        
                        //run info through neural network (slightly larger than mf network.
                        //update object style parameter
                    }
                    //profit
                }
                catch(Exception e)
                {
                    e.OutputError();
                }
            }

        }
        public ObjStyleUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("0df8fb78-134b-4d6d-a14e-15a70ac2de12"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Updates an Elements Object Style"; }
        public ChangePriority GetChangePriority() { return ChangePriority.FreeStandingComponents; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update Object Styles"; }

    }
}
