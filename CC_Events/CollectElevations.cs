using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using CC_Library;
using CC_Library.Datatypes;

namespace CC_Plugin
{
    internal static class CollectElevations
    {
        public static void ShowElement(this Document doc, UIDocument uidoc)
        {
            Selection sel = uidoc.Selection;
            var selIds = uidoc.Selection.GetElementIds();
            foreach(ElementId eid in selIds)
            {
                Element e = doc.GetElement(eid);
                TaskDialog.Show("Test", e.Category.Name);
            }

        }
        public static double[] CollectBounds(this Document doc, UIDocument uidoc)
        {
            List<double> room = new List<double>();
            List<double> bounds = new List<double>();
            Selection sel = uidoc.Selection;
            bool RoomFound = false;
            var selIds = uidoc.Selection.GetElementIds();
            foreach (ElementId eid in selIds)
            {
                Element e = doc.GetElement(eid);
                DetailCurve dc = e as DetailCurve;
                if (dc != null)
                {
                    if(!RoomFound)
                    {
                        XYZ mid = dc.GeometryCurve.Evaluate(0.5, true);
                        var r = doc.GetRoomAtPoint(mid);
                        if(r != null)
                        {
                            RoomFound = true;
                            IList<IList<BoundarySegment>> segments = r.GetBoundarySegments(new SpatialElementBoundaryOptions());
                            if (null != segments)
                            {
                                foreach (IList<BoundarySegment> SegmentList in segments)
                                {
                                    for (int i = 0; i < SegmentList.Count; i++)
                                    {
                                        XYZ bp = SegmentList[i].GetCurve().GetEndPoint(0);
                                        room.Add(bp.X);
                                        room.Add(bp.Y);
                                    }
                                }
                            }
                        }
                    }
                    XYZ start = dc.GeometryCurve.GetEndPoint(0);
                    XYZ end = dc.GeometryCurve.GetEndPoint(1);
                    bounds.Add(start.X);
                    bounds.Add(start.Y);
                    bounds.Add(end.X);
                    bounds.Add(end.Y);
                }
            }
            int roomcount = room.Count();
            for(int i = 0; i < 201; i++)
            {
                if (i > roomcount)
                    room.Add(0);
            }
            for(int i = 0; i < 51; i++)
            {
                if (i > bounds.Count())
                    bounds.Add(0);
            }
            room.AddRange(bounds);

            double max = Math.Abs(room.Max());
            double min = Math.Abs(room.Min());
            if (min > max)
                max = min;

            for (int i = 0; i < room.Count(); i++)
                room[i] /= max;

            return room.ToArray();
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ShowCategory : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            var bounds = doc.CollectBounds(uiDoc);
            List<string> Lines = new List<string>();
            string s = bounds[0].ToString();
            for(int i = 1; i < bounds.Count(); i++)
            {
                s += "," + bounds[i];
            }
            Lines.Add(s);

            string fn = null;
            while (fn == null)
            {
                fn = Files.GetFile();
            }
            File.AppendAllLines(fn + "_Elevation.csv", Lines);

            return Result.Succeeded;
        }
    }
}
