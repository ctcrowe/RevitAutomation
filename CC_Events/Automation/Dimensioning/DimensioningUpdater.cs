using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public class Dimensioner
    {
        public static void OnStartup(UIControlledApplication application)
        {
            DimensionParamUpdater updater = new DimensionParamUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            
            ElementClassFilter refFilter = new ElementClassFilter(typeof(ReferencePlane));
            
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), refFilter, Element.GetChangeTypeElementAddition());
        }

        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            DimensionParamUpdater updater = new DimensionParamUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
    }

    public class DimensionParamUpdater : IUpdater
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";
        private static List<Param> Parameters = new List<Param> {
            ParameterLibrary.OverallDepth, ParameterLibrary.OverallHeight, ParameterLibrary.OverallWidth,
            ParameterLibrary.FDataCat, ParameterLibrary.FDataGeom};

        static AddInId appId;
        static UpdaterId updaterId;

        public DimensionParamUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("15db3b5a-0bb3-4673-93ff-0db8755cfdda"));
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            if (doc.IsFamilyDocument)
            {
                ResetParamLibrary.Run();


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
        }
        public string GetAdditionalInformation() { return "Adds Typical Dimensioning Parameters to the Project"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Dimension Parameter Updater"; }
    }
}