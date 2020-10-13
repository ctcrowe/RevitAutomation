using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using CC_Library;
using CC_Library.Datatypes;

namespace CC_Plugin
{
    internal static class CollectBoundaries
    {
        public static List<double[]> CollectBounds(this Room r)
        {
            List<double[]> bounds = new List<double[]>();
            IList<IList<BoundarySegment>> segments = r.GetBoundarySegments(new SpatialElementBoundaryOptions());
            if (null != segments)
            {
                foreach (IList<BoundarySegment> SegmentList in segments)
                {
                    for (int i = 0; i < SegmentList.Count; i++)
                    {
                        XYZ bp = SegmentList[i].GetCurve().GetEndPoint(0);
                        bounds.Add(new double[3] { bp.X, bp.Y, bp.Z });
                    }
                }
            }
            return bounds;
        }
        public static double[] GetBound(this List<double[]> Bounds, int i)
        {
            if (i > 0 && i < Bounds.Count())
                return Bounds[i];
            if (i < 0 && i > -1 * Bounds.Count())
                return Bounds[Bounds.Count() + i];
            if (i > Bounds.Count() && i < 2 * Bounds.Count())
                return Bounds[i - Bounds.Count()];
            return new double[3] { 0, 0, 0 };
        }
        public static double[] Direction(this double[] B1, double[] B2)
        {
            var direction = new double[3] { B2[0] - B1[0], B2[1] - B1[1], B2[2] - B1[2] };
            return direction.Unitize();
        }
        public static  double[] Unitize(this double[] L)
        {
            return new double[3] { L[0] / L.Max(), L[1] / L.Max(), L[2] / L.Max() };
        }
        public static double UnitLength(this double[] L)
        {
            return Math.Sqrt(Math.Pow(L[0], 2) + Math.Pow(L[1], 2) + Math.Pow(L[2], 2));
        }
        public static double AngleBetween(this double[] L1, double[] L2)
        {
            double theta = (L1[0] * L2[0]) + (L1[1] * L2[1]) + (L1[2] * L2[2]);
            double t1 = L1.UnitLength() * L2.UnitLength();
            return Math.Acos(theta / t1);
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CollectRoomBounds : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            int boundcount = 0;
            List<Element> rooms = new FilteredElementCollector(doc, uiDoc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).ToElements().ToList();
            List<string> Lines = new List<string>();
            foreach (Element e in rooms)
            {
                Room r = e as Room;
                var bounds = r.CollectBounds();
                if (bounds.Count() > boundcount)
                    boundcount = bounds.Count();
                for (int i = 0; i < bounds.Count(); i++)
                {
                    string result = "Y";
                    var b1 = bounds.GetBound(i - 2);
                    var b2 = bounds.GetBound(i - 1);
                    var b3 = bounds.GetBound(i);
                    var b4 = bounds.GetBound(i + 1);
                    var b5 = bounds.GetBound(i + 2);
                    var D1 = b2.Direction(b3);
                    var D2 = b3.Direction(b4);
                    var D3 = b1.Direction(b2);
                    var D4 = b4.Direction(b5);
                    if (D1.AngleBetween(D2) < 0.0872665)
                        result = "N";
                    if(D1.UnitLength() < 3)
                    {
                        if (D3.AngleBetween(D2) < 0.0872665)
                            result = "N";
                    }
                    if (D2.UnitLength() < 3)
                    {
                        if (D1.AngleBetween(D4) < 0.0872665)
                            result = "N";
                    }
                    string s = b1[0] + "," + b1[1] + "," + b1[2] + "," +
                        b2[0] + "," + b2[1] + "," + b2[2] + "," +
                        b3[0] + "," + b3[1] + "," + b3[2] + "," +
                        b4[0] + "," + b4[1] + "," + b4[2] + "," +
                        b5[0] + "," + b5[1] + "," + b5[2] + "," + result;
                    Lines.Add(s);
                }
            }
            string fn = null;
            while (fn == null)
            {
                fn = GetFileLocation.GetFile();
            }
            TaskDialog.Show("Max Bounds", boundcount.ToString());
            File.WriteAllLines(fn + "_Boundary.csv", Lines);

            return Result.Succeeded;
        }
    }
}
