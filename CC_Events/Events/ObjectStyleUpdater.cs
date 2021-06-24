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
        private double[] GetDims(BoundingBox bbox)
        {
            double[] dims = new double[18];
            SetArray(dims, 
            dims[0] = bbox.Min.X;
            dims[1] = bbox.Min.Y;
            dims[2] = bbox.Min.Z;
            dims[3] = bbox.Max.X;
            dims[4] = bbox.Max.Y;
            dims[5] = bbox.Max.Z;
            dims[6] = bbox.Transform.BasisX.X;
            dims[7] = bbox.Transform.BasisX.Y;
            dims[8] = bbox.Transform.BasisX.Z;
            dims[9] = bbox.Transform.BasisY.X;
            dims[10] = bbox.Transform.BasisY.Y;
            dims[11] = bbox.Transform.BasisY.Z;
            dims[12] = bbox.Transform.BasisZ.X;
            dims[13] = bbox.Transform.BasisZ.Y;
            dims[14] = bbox.Transform.BasisZ.Z;
            dims[15] = bbox.Transform.Origin.X;
            dims[16] = bbox.Transform.Origin.Y;
            dims[17] = bbox.Transform.Origin.Z;
            return dims;
        }
        private double[] SetArray(double[] a, XYZ pt, int s)
        {
            if(a.Count() >= s + 3)
            {
                a[s] = pt.X;
                a[s + 1] = pt.Y;
                a[s + 2] = pt.Z;
            }
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
                        var dims = GetDims(ele.BoundingBox);
                        var bbox = ele.BoundingBox;
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
