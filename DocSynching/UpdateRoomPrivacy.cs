using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

using CC_Library.Parameters;
using CC_Library.Predictions.RoomPrivacy;
using CC_RevitBasics;

namespace CC_DocSynching
{
    internal static class CMDUpdateRoomPrivacy
    {
        public static void UpdateRoomPrivacy(this View view)
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
                        if (r.Name.Split(' ').FirstOrDefault() != "Room")
                        {
                            string s = e.GetElementParam(CCParameter.RoomPrivacy);
                            bool test = false;
                            if (s == null)
                                test = true;
                            if (s == "")
                                test = true;
                            if(test)
                            {
                                string privacy = RoomPrivacy.Predict(r.Name);
                                if (privacy != null)
                                    r.SetRoomTextParam(CCParameter.RoomPrivacy, privacy);
                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
