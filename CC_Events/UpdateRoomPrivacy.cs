using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

using CC_Library.Parameters;
using CC_Library.Datatypes;
using CC_Library.Predictions;
using CC_RevitBasics;

namespace CC_Plugin
{
    internal static class CMDUpdateRoomPrivacy
    {
        public static void UpdateRoomPrivacy(this View view)
        {
            if(view.ViewType == ViewType.FloorPlan)
            {
                List<Element> Rooms = new FilteredElementCollector(view.Document, view.Id)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .ToList();
                foreach(Element e in Rooms)
                {
                    try
                    {
                        Room r = e as Room;
                        if (r.Name.Split(' ').FirstOrDefault() != "Room")
                        {
                            if (string.IsNullOrEmpty(e.GetElementParam(CCParameter.RoomPrivacy)))
                            {
                                string privacy = Datatype.RoomPrivacy.FindClosest(r.Name);
                                if (privacy != null)
                                {
                                    r.SetRoomIntParam(CCParameter.RoomPrivacy, privacy);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
