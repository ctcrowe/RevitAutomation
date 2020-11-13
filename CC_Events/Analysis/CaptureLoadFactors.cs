using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CC_RevitBasics;
using System.IO;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{
    public static class CaptureRoomLoadFactors
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static void CaptureLoadFactor(this Document doc)
        {
            List<Element> RoomCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToList();
            string filename = directory + "\\FOUNDLABELS_OccupantLoadFactor.csv";
            List<string> lines = new List<string>();

            foreach (Element e in RoomCollector)
            {
                Room r = e as Room;
                string name = r.Name;
                string LoadFactor = e.GetElementParam(CC_Library.Parameters.CCParameter.OccupantLoadFactor);
                lines.Add(name + ',' + LoadFactor);
            }

            File.WriteAllLines(filename, lines);
        }
    }
}
