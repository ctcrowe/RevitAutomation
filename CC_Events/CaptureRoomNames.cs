using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CC_Library;
using CC_RevitBasics;
using System.IO;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{
    public static class CaptureRoomNames
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string dir = directory + "\\CC_ElesByID";

        public static void CaptureLoadFactor(this Document doc)
        {
            List<Element> RoomCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToList();
            string subdir = dir + "\\" + doc.Application.VersionNumber.ToString();
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
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CollectRooms : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            doc.CaptureLoadFactor();
            return Result.Succeeded;
        }
    }
}
