using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Creation;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace CC_Plugin
{

    internal class BoundaryData
    {
        Room room;
        public BoundaryData(Room roomfordata)
        {
            this.room = roomfordata;
        }
        private double maxangle = Math.PI * 5 / 180;
        public List<XYZ> BasePoints()
        {
            IList<IList<BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
            List<XYZ> bp = new List<XYZ>();
            if (null != segments)
            {
                foreach (IList<BoundarySegment> SegmentList in segments)
                {
                    for (int i = 0; i < SegmentList.Count; i++)
                    {
                        bp.Add(SegmentList[i].GetCurve().GetEndPoint(0));
                    }
                }
            }
            return bp;
        }
        private bool LineRelation(XYZ i, XYZ j, XYZ k)
        {
            bool LC = true;
            XYZ l = k - j;
            if (l.AngleTo(i) > maxangle)
            {
                if (l.AngleTo(i) < Math.PI - maxangle)
                {
                    LC = false;
                }
            }
            return LC;
        }
        public Dictionary<Line, XYZ> Edges()
        {
            Dictionary<Line, XYZ> e = new Dictionary<Line, XYZ>();

            //get first point
            //get line from first point to second point = l1
            //get line from first point to last point = ln
            //compare their direction, If it is consistent, look at line from last point to second to last (l n-1). repeat.
            //get line from second point to third point = l2
            //compare direction from l1 and l2. if direction is similar or if l2 is short enough move on to l3.
            //When the direction diverges, end the line, begin a new one (l3 = third point to fourth point)
            if (BasePoints().Count >= 2)
            {
                int i = 0;
                int j = BasePoints().Count - 1;
                while (i < j)
                {
                    XYZ ipt = BasePoints()[i + 1] - BasePoints()[i];

                    XYZ up = XYZ.BasisZ;
                    XYZ rd = ipt.CrossProduct(up);
                    XYZ angle = rd.Normalize();
                    bool EoL = false;
                    int k = i;
                    int l = i + 1;

                    if (i == 0)
                    {
                        bool BoL = false;
                        if (LineRelation(ipt, BasePoints()[j], BasePoints()[0]))
                        {
                            while (BoL == false)
                            {
                                k = j;
                                if (j >= 1)
                                {
                                    if (LineRelation(ipt, BasePoints()[j - 1], BasePoints()[j]))
                                    {
                                        j--;
                                    }
                                    else
                                    {
                                        BoL = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            XYZ jpt = BasePoints()[0] - BasePoints()[j];
                            if (!LineRelation(jpt, BasePoints()[j - 1], BasePoints()[j]))
                            {
                                XYZ jAngle = jpt.CrossProduct(up).Normalize();
                                e.Add(Line.CreateBound(BasePoints()[j], BasePoints()[0]), jAngle);
                            }
                        }
                    }
                    while (EoL == false)
                    {
                        if (i < BasePoints().Count() - 1)
                        {
                            if (LineRelation(ipt, BasePoints()[i], BasePoints()[i + 1]))
                            {
                                i++;
                                l = i;
                            }
                            else
                            { EoL = true; }
                        }
                        else
                        {
                            if (LineRelation(ipt, BasePoints()[i], BasePoints()[0]))
                            {
                                l = 0;
                            }
                            EoL = true;
                        }
                    }
                    e.Add(Line.CreateBound(BasePoints()[k], BasePoints()[l]), angle);
                }
            }
            return e;
        }
        public List<Line> EdgeLines()
        {
            List<Line> a = new List<Line>();
            int i = 0;
            for (i = 0; i < BasePoints().Count - 1; i++)
            {
                Line l = Line.CreateBound(BasePoints()[i], BasePoints()[i + 1]);
                a.Add(l);
            }
            Line b = Line.CreateBound(BasePoints().Last(), BasePoints()[0]);
            a.Add(b);
            return a;
        }
    }
    internal static class FinishPlan
    {
        public static void CreateFinFloor(this Room room)
        {
            BoundaryData BData = new BoundaryData(room);
            if (null != BData.Edges())
            {
                CurveArray array = new CurveArray();
                foreach (KeyValuePair<Line, XYZ> kvp in BData.Edges())
                {
                    Curve c = kvp.Key as Curve;
                    array.Append(c);
                }
                Autodesk.Revit.Creation.Document doc = room.Document.Create;
                doc.NewFloor(array, false);
            }
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class GenerateFinishFloors : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Autodesk.Revit.DB.Document currentDoc = uiapp.ActiveUIDocument.Document;
            View currentView = currentDoc.ActiveView;

            List<Element> Rooms = new FilteredElementCollector(currentDoc, currentView.Id).OfCategory(BuiltInCategory.OST_Rooms).ToList();
            using (Transaction t = new Transaction(currentDoc, "Finish Floors"))
            {
                t.Start();
                foreach(Element e in Rooms)
                {
                    Room r = e as Room;
                    r.CreateFinFloor();
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
