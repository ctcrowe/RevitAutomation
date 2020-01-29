using System;
using Autodesk.Revit.DB;

namespace CC_Plugin
{

    internal class MasterFormatUpdater : IUpdater
    {
        public void Execute(UpdaterData data)
        {
        }
        public static void ProjectStartup(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                MasterFormatUpdater updater = new MasterFormatUpdater(doc.Application.ActiveAddInId);
                UpdaterRegistry.RegisterUpdater(updater, true);
                ElementClassFilter filt = new ElementClassFilter(typeof(FamilyInstance));

                FamilyParameter p =  doc.FamilyManager.get_Parameter(MFParam.ID);

                UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filt, Element.GetChangeTypeParameter(p.Id));
            }
        }
        public static void ProjectShutdown(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                MasterFormatUpdater updater = new MasterFormatUpdater(doc.Application.ActiveAddInId);
                UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            }
        }
        public MasterFormatUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("1aaf52be-12f5-439b-a710-4a23245d6915"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Standard Automation Loop Used By Charlie Crowe"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "CC Automation Loop"; }
    }
}
