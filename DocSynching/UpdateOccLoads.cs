using System.Collections.Generic;
using System.Linq;
using System;
using CC_Library.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using CC_Library.Predictions;
using CC_RevitBasics;

namespace CC_DocSynching
{
    internal static class UpdateOccupantLoads
    {
        public static void UpdateOccLoads(this View view)
        {
            if (view.ViewType == ViewType.FloorPlan)
            {
                List<Element> Rooms = new FilteredElementCollector(view.Document, view.Id)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .ToList();
                foreach (Element e in Rooms)
                {
                    try
                    {
                        Room r = e as Room;
                        double lf = double.Parse(e.GetElementParam(CCParameter.OccupantLoadFactor).Split(' ').First());
                        double Area = r.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
                        int load = 0;
                        if (lf > 0)
                            load = (int)Math.Ceiling(Area / lf);
                        e.SetElementParam(CCParameter.OccupantLoad, load.ToString());
                    }
                    catch { }
                }
            }
        }
    }
}
