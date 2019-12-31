using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public class RoomParameterizer
    {
        public static void OnStartup(UIControlledApplication application)
        {
            RoomParamUpdater updater = new RoomParamUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);

            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());
        }

        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            RoomParamUpdater updater = new RoomParamUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
    }

    public class RoomParamUpdater : IUpdater
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";
        private static List<Param> Parameters = new List<Param> { ParameterLibrary.Privacy };

        static AddInId appId;
        static UpdaterId updaterId;

        public RoomParamUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("eb3c57fe-182d-4329-84ad-8db02b560cdf"));
        }
        public void Execute(UpdaterData data)
        {
            ResetParamLibrary.Run();

            Document doc = data.GetDocument();

            Application App = doc.Application;
            App.SharedParametersFilename = SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();

            for (int i = 0; i < Parameters.Count; i++)
            {
                try
                {
                    Definition def = AddParam.SetupParam(deffile, doc, Parameters[i]);
                    AddParam.Run(def, doc, Parameters[i]);
                }
                catch
                { }
            }
        }
        public string GetAdditionalInformation() { return "Adds Typical Room and Space Parameters to the Project"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Room Parameter Updater"; }
    }
}