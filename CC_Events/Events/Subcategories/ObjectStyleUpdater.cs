using Autodesk.Revit.DB;
using System.Linq;
using System;
using Autodesk.Revit.UI;

using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;

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
                Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(GenericForm)),
                Element.GetChangeTypeElementAddition());
        }
        private double[] GetDims(BoundingBoxXYZ bbox)
        {
            double[] dims = new double[18];
            dims = SetArray(dims, bbox.Min, 0);
            dims = SetArray(dims, bbox.Max, 3);
            dims = SetArray(dims, bbox.Transform.BasisX, 6);
            dims = SetArray(dims, bbox.Transform.BasisY, 9);
            dims = SetArray(dims, bbox.Transform.BasisZ, 12);
            dims = SetArray(dims, bbox.Transform.Origin, 15);
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
            return a;
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            if (doc.IsFamilyDocument)
            {
                try
                {
                    //Get document Name
                    string name = doc.Title;
                    if(String.IsNullOrEmpty(name))
                        name = "null";

                    //Get form location info (Varies by type?)
                    foreach (ElementId e in data.GetModifiedElementIds().Concat(data.GetAddedElementIds()))
                    {
                        GenericForm ele = doc.GetElement(e) as GenericForm;
                        var bbox = ele.get_BoundingBox(null);
                        if (bbox != null)
                        {
                            var dims = GetDims(bbox);
                            int prediction = Datatype.ObjectStyle.PredictSingle(name, "null", dims);
                            var subcat = doc.AddCategories(prediction);
                            ele.Subcategory = subcat;
                        }
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
