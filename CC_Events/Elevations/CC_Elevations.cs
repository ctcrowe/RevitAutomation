using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace CC_Events
{
    /// <summary>
    /// new process
    /// collect all boundaries for a room
    /// create a list of unbound lines that run through those boundary conditions
    /// reduce that list to a logical set that defines elevaitons
    /// compare that set against the overall boundary conditions for the room.
    /// if that list is 4 or less, create one set of elevations in thet center of the room.
    /// rotate that set to reorient it
    /// if the list is more than 4, generate elevations basically as is. Maybe look to combine?
    /// </summary>
    internal class GenerateViews
    {
        public static ViewFamilyType ViewTypeSetup(Document currentDoc)
        {
            ViewFamilyType vf1 = new FilteredElementCollector(currentDoc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x => ViewFamily.Elevation == x.ViewFamily);

            ViewFamilyType vft;

            FilteredElementCollector vftfilter = new FilteredElementCollector(currentDoc).OfClass(typeof(ViewFamilyType));
            ViewFamilyType vcheck = vftfilter.FirstOrDefault<Element>(q => q.Name.Equals("Generated Interior Elevation")) as ViewFamilyType;
            if(vcheck == null)
            {
                vft = vf1.Duplicate("Generated Interior Elevation") as ViewFamilyType;
            }
            else
            {
                if(vcheck.Name != "Generated Interior Elevation")
                {
                    vft = vf1.Duplicate("Generated Interior Elevation") as ViewFamilyType;
                }
                else
                {
                    vft = vcheck;
                }
            }
            return vft;
        }
        private static double offset = 1.5;
        private static XYZ up = XYZ.BasisZ;
        private static string Direction(double angle)
        {
            int calcedangle = (int)Math.Round((angle * 180) / (Math.PI * 45));
            switch(calcedangle)
            {
                case -3:
                    return "SE";
                case -2:
                    return "E";
                case -1:
                    return "NE";
                case 1:
                    return "NW";
                case 2:
                    return "W";
                case 3:
                    return "SW";
                case 4:
                case -4:
                    return "S";
                case 0:
                default:
                    return "N";
            }
        }
        public static List<Line> SigRoomBounds(Room room)
        {
            StringBuilder sb = new StringBuilder();
            IList<IList<BoundarySegment>> segs = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
            List<Line> lines = new List<Line>();
            if(null != segs)
            {
                foreach(IList<BoundarySegment> SegmentList in segs)
                {
                    for(int i = 0; i < SegmentList.Count(); i++)
                    {
                        lines.Add(Line.CreateUnbound(SegmentList[i].GetCurve().GetEndPoint(0), SegmentList[i].GetCurve().GetEndPoint(1) - SegmentList[i].GetCurve().GetEndPoint(0)));
                    }
                }
            }
            for(int i = 0; i < lines.Count(); i++)
            {
                for(int j = 0; j < lines.Count(); j++)
                {
                    if(j != i)
                    {
                        if(lines[i].Direction == lines[j].Direction || lines[i].Direction == -lines[j].Direction)
                        {
                            if(lines[i].Distance(lines[j].Origin) <= offset)
                            {
                                lines.RemoveAt(j);
                            }
                        }
                    }
                }
            }
            return lines;
        }
        public bool Is4Sided(List<Line> edges)
        {
            if(edges.Count() <= 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static List<ElementId> ViewsToDelete(Room r)
        {
            Document currentDoc = r.Document;
            List<ElementId> VTDs = new List<ElementId>();

            string vname = r.Name;

            FilteredElementCollector sheetCollector = new FilteredElementCollector(currentDoc);
            sheetCollector.OfCategory(BuiltInCategory.OST_Sheets);
            FilteredElementCollector viewfilter = new FilteredElementCollector(currentDoc).OfClass(typeof(View));
            foreach (View v in viewfilter)
            {
                if (Viewport.CanAddViewToSheet(currentDoc, sheetCollector.FirstElement().Id, v.Id) != false)
                {
                    if (v.Name.Split('-').First() == vname + ' ')
                    {
                        VTDs.Add(v.Id);
                    }
                }
            }
            return VTDs;
        }
        public static void GenViews(ViewFamilyType vft, View av, Room r)
        {
            Document currentDoc = r.Document;

            foreach (ElementId vtd in ViewsToDelete(r))
            {
                if (currentDoc.GetElement(vtd).IsValidObject)
                {
                    currentDoc.Delete(vtd);
                }
            }

            BoundingBoxXYZ bbse = r.get_BoundingBox(null);
            double yMin = bbse.Min.Z;
            double yMax = bbse.Max.Z;

            BoundaryData BData = new BoundaryData(r);
            if (null != BData.Edges())
            {
                foreach (KeyValuePair<Line, XYZ> kvp in BData.Edges())
                {
                    XYZ start = kvp.Key.GetEndPoint(0);
                    XYZ end = kvp.Key.GetEndPoint(1);
                    XYZ vector = end - start;
                    XYZ midpt = start + (0.5 * vector);

                    double xsize = kvp.Key.Length;
                    double height = yMax - yMin;

                    double minx = midpt.X - (xsize / 2) - offset;
                    double maxx = midpt.X + (xsize / 2) + offset;
                    double miny = yMin - offset;
                    double maxy = yMax + offset;
                    double minz = midpt.Z - offset / 2;
                    double maxz = midpt.Z + offset / 2;

                    XYZ minbound = new XYZ(minx, miny, minz);
                    XYZ maxbound = new XYZ(maxx, maxy, maxz);

                    BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
                    sectionBox.Min = minbound;
                    sectionBox.Max = maxbound;

                    ElevationMarker em = ElevationMarker.CreateElevationMarker(currentDoc, vft.Id, midpt, 48);
                    ViewSection vs = em.CreateElevation(currentDoc, av.Id, 1);
                    vs.CropBox = sectionBox;

                    XYZ baseangle = XYZ.BasisY;
                    double rotation = kvp.Value.AngleTo(baseangle);
                    if (kvp.Value.X > 0)
                    {
                        rotation *= -1;
                    }

                    string dir = Direction(rotation);

                    Line axisline = Line.CreateBound(midpt, new XYZ(midpt.X, midpt.Y, midpt.Z + 1));
                    em.Location.Rotate(axisline, rotation);

                    em.Location.Move(-kvp.Value * offset);

                    int j = 1;
                    bool iscreated = false;
                    while (iscreated == false)
                    {
                        string viewname = r.Name + " - " + dir + " " + j;
                        FilteredElementCollector vfilter = new FilteredElementCollector(currentDoc).OfClass(typeof(View));
                        View testname = vfilter.FirstOrDefault<Element>(a => a.Name.Equals(viewname)) as View;
                        if (testname == null)
                        {
                            vs.Name = viewname;
                            iscreated = true;
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
            }
        }
    }
    internal class BoundaryData
    {
        Room r;
        public BoundaryData(Room i)
        {
            this.r = i;
        }
        private double minlength = 1.5;
        private double maxangle = Math.PI * 5 / 180;
        public List<XYZ> BasePoints()
        {
            IList<IList<BoundarySegment>> segments = r.GetBoundarySegments(new SpatialElementBoundaryOptions());
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
            if (l.GetLength() > minlength)
            {
                if (l.AngleTo(i) > maxangle)
                {
                    if (l.AngleTo(i) < Math.PI - maxangle)
                    {
                        LC = false;
                    }
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
            //compare there direction, If it is consistent, look at line from last point to second to last (l n-1). repeat.
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
                    if (ipt.GetLength() > minlength)
                    {
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
                    else
                    {
                        i++;
                    }
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
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CreateFloorViews : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document currentDoc = uiApp.ActiveUIDocument.Document;
            View av = currentDoc.ActiveView;

            ViewFamilyType vft = GenerateViews.ViewTypeSetup(currentDoc);

            if (av.ViewType == ViewType.FloorPlan)
            {
                RoomFilter filt = new RoomFilter();
                ICollection<Element> roomset = new FilteredElementCollector(currentDoc, av.Id).WherePasses(filt).ToElements();
                foreach (Element e in roomset)
                {
                    Room r = e as Room;
                    GenerateViews.GenViews(vft, av, r);
                }
            }
            else
            {
                TaskDialog.Show("Error", "Active a plan view!");
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CreateRoomViews : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document currentDoc = uiApp.ActiveUIDocument.Document;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            View av = currentDoc.ActiveView;

            ViewFamilyType vft = GenerateViews.ViewTypeSetup(currentDoc);

            if (av.ViewType == ViewType.FloorPlan)
            {
                Selection sel = uiDoc.Selection;
                ICollection<ElementId> set = sel.GetElementIds();

                if (set.Count == 1)
                {
                    foreach (ElementId id in set)
                    {
                        Element e = currentDoc.GetElement(id);
                        Room r = e as Room;
                        if (null != r)
                        {
                            BoundaryData b = new BoundaryData(r);
                            GenerateViews.GenViews(vft, av, r);
                        }
                        else
                        {
                            TaskDialog.Show("Error", "Please select a room.");
                        }
                    }
                }
                else
                {
                    TaskDialog.Show("Error", "Please select a room.");
                }
            }
            else
            {
                TaskDialog.Show("Error", "Active a plan view!");
            }
            return Result.Succeeded;
        }
    }
}