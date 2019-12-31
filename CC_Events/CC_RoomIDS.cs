using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{
    internal class RoomID
    {
        public static void SetID(Document doc)
        {
            try
            {
                List<Element> rooms = new FilteredElementCollector(doc).
                    OfCategory(BuiltInCategory.OST_Rooms).ToElements().ToList();

                foreach (Element r in rooms)
                {
                    IDParam.Set(r as Room);
                }
            }
            catch { }
        }
    }
}