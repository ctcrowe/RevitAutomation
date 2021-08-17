using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;

using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;
using CC_Library.Parameters;

using CC_Plugin.Parameters;

namespace CC_Plugin
{
    internal class LineStyleUpdater : IUpdater
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            RegisterUpdater(app.ActiveAddInId);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            LineStyleUpdater updater = new LineStyleUpdater(app.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }
        public static void RegisterUpdater(AddInId id)
        {
            LineStyleUpdater updater = new LineStyleUpdater(id);
            UpdaterRegistry.RegisterUpdater(updater, true);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(CurveElement)),
                Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(CurveElement)),
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
                        CurveElement ele = doc.GetElement(e) as CurveElement;
                        var bbox = ele.get_BoundingBox(null);
                        if (bbox != null)
                        {
                            var dims = GetDims(bbox);
                            int prediction = Datatype.ObjectStyle.PredictSingle(name, "", dims);
                            var subcat = doc.AddCategories(prediction);
                            ele.LineStyle = subcat.GetGraphicsStyle(GraphicsStyleType.Projection);
                        }
                    }
                }
                catch(Exception e)
                {
                    e.OutputError();
                }
            }

        }
        public LineStyleUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("fac9403f-393e-4544-b787-3877aa0351bf"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Updates an Elements Line Style"; }
        public ChangePriority GetChangePriority() { return ChangePriority.FreeStandingComponents; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update Line Styles"; }
    }
}
