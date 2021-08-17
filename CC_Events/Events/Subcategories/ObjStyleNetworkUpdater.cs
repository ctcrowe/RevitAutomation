using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;

using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;
using CC_Library.Parameters;

using CC_Plugin.Parameters;

namespace CC_Plugin
{
    internal class ObjStyleNetworkUpdater : IUpdater
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            RegisterUpdater(app.ActiveAddInId);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            ObjStyleNetworkUpdater updater = new ObjStyleNetworkUpdater(app.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }
        public static void RegisterUpdater(AddInId id)
        {
            ObjStyleNetworkUpdater updater = new ObjStyleNetworkUpdater(id);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementId pid = new ElementId(BuiltInParameter.FAMILY_ELEM_SUBCATEGORY);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(GenericForm)),
                Element.GetChangeTypeParameter(pid));
        }
        private static string Write(string s)
        {
            TaskDialog.Show("CHECKING ERROR", s);
            return s;
        }
        private static string WriteNull(string s)
        {
            return s;
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
            if (a.Count() >= s + 3)
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
                    foreach (ElementId e in data.GetModifiedElementIds())
                    {
                        GenericForm ele = doc.GetElement(e) as GenericForm;
                        var bbox = ele.get_BoundingBox(null);
                        if (bbox != null)
                        {
                            var dims = GetDims(bbox);
                            int Correct = Enum.GetNames(typeof(ObjectCategory)).ToList().IndexOf(Enum.GetNames(typeof(ObjectCategory)).ToList().Where(x => ele.Subcategory.Name.Contains(x)).First());
                            Datatype.ObjectStyle.PropogateSingle(Correct, new WriteToCMDLine(WriteNull), name, "", dims);
                        }
                        //run info through neural network (slightly larger than mf network.
                        //update object style parameter
                    }
                    //profit
                }
                catch (Exception e)
                {
                    e.OutputError();
                }
            }

        }
        public ObjStyleNetworkUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("af950c46-2b41-43d8-b224-79906787ff6c"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Updates an Elements Object Style"; }
        public ChangePriority GetChangePriority() { return ChangePriority.FreeStandingComponents; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update Object Styles"; }

    }
}
