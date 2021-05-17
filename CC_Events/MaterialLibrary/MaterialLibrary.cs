using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CC_Library.Parameters;
using CC_Plugin.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_RevitBasics;
using CC_Library.Materials;

namespace CC_Plugin
{
    internal class MaterialLibrary : IUpdater
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public void Execute(UpdaterData data)
        {
            List<ElementId> eids = data.GetModifiedElementIds().ToList();
            Document doc = data.GetDocument();

            foreach (ElementId eid in eids)
            {
                Element ele = doc.GetElement(eid) as Material;
                var cat = ele.GetElementParam(MaterialParams.Category);
                if (string.IsNullOrEmpty(cat) || string.IsNullOrWhiteSpace(cat))
                    cat = "null";
                var mdl = ele.GetElementParam(MaterialParams.Model);
                if (string.IsNullOrEmpty(mdl) || string.IsNullOrWhiteSpace(mdl))
                    mdl = "null";
                var mfr = ele.GetElementParam(MaterialParams.Manufacturer);
                if (string.IsNullOrEmpty(mfr) || string.IsNullOrWhiteSpace(mfr))
                    mfr = "null";
                var ptn = ele.GetElementParam(MaterialParams.PatternStyle);
                if (string.IsNullOrEmpty(ptn) || string.IsNullOrWhiteSpace(ptn))
                    ptn = "null";
                var clr = ele.GetElementParam(MaterialParams.ColorFinish);
                if (string.IsNullOrEmpty(clr) || string.IsNullOrWhiteSpace(clr))
                    clr = "null";
                var dsc = ele.GetElementParam(MaterialParams.MatDescription);
                if (string.IsNullOrEmpty(dsc) || string.IsNullOrWhiteSpace(dsc))
                    dsc = "null";
                var prj = doc.ProjectInformation.Number;
                if (string.IsNullOrEmpty(prj) || string.IsNullOrWhiteSpace(prj))
                    prj = "null";

                CCMaterial mat = CCMaterial.Get(cat, mdl);
                mat.Manufacturer = mfr;
                mat.Color = clr;
                mat.Description = dsc;

                mat.AddPattern(ptn);
                mat.AddProject(prj);
                
                mat.Save(); 
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            MaterialLibrary updater = new MaterialLibrary(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementClassFilter mats = new ElementClassFilter(typeof(Material));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), mats, Element.GetChangeTypeAny());
        }
        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            MaterialLibrary updater = new MaterialLibrary(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        public MaterialLibrary(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("60cb500d-4c01-4c72-bfee-cf5f80d48386"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return ""; }
        public ChangePriority GetChangePriority() { return ChangePriority.DetailComponents; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update Material Library"; }
    }
}