using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Parameters;

using Autodesk.Revit.DB;
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

    internal class UpdateStudSize : IUpdater
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public void Execute(UpdaterData data)
        {
            List<ElementId> eids = data.GetModifiedElementIds().ToList();
            Document doc = data.GetDocument();

            foreach (ElementId eid in eids)
            {
        
                double thickness = 0;
                Element ele = doc.GetElement(eid);
                WallType wt = ele as WallType;
                if (wt.Kind == WallKind.Basic)
                {
                    var layers = wt.GetCompoundStructure().GetLayers();

                    string[] s = new string[layers.Count()];
                    for(int i = 0; i < layers.Count(); i++)
                    {
                        var materialId = layers[i].MaterialId;
                        if (materialId != ElementId.InvalidElementId)
                        {
                            Material mat = doc.GetElement(layers[i].MaterialId) as Material;
                            s[i] = mat.Name;
                        }
                        else
                        {
                            s[i] = "NullMaterial";
                        }
                    }
                    string lyr = s.PredictStudSize();
                    foreach (var layer in layers)
                    {
                        var matId = layer.MaterialId;
                        if (matId != ElementId.InvalidElementId)
                        {
                            Material mat = doc.GetElement(layer.MaterialId) as Material;
                            string a = mat.Name;
                            if (a == lyr)
                            {
                                if (thickness < layer.Width)
                                    thickness = layer.Width;
                            }
                        }
                        else
                        {
                            if (lyr == "NullMaterial")
                                if (thickness < layer.Width)
                                    thickness = layer.Width;
                        }
                    }
                }
                wt.SetWallTypeParam(CCParameter.StudSize, thickness.ToString());
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            UpdateStudSize updater = new UpdateStudSize(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementClassFilter walls = new ElementClassFilter(typeof(WallType));
            ElementId width = new ElementId(BuiltInParameter.WALL_ATTR_WIDTH_PARAM);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), walls, Element.GetChangeTypeParameter(width));
        }
        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            UpdateStudSize updater = new UpdateStudSize(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        public UpdateStudSize(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("719fe0bd-efa3-4db3-9dc0-95eea3003765"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return ""; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update Stud Size"; }
    }
}