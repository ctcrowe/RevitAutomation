using System.Linq;
using System;
using CC_Library;
using CC_Library.Parameters;
using CC_Library.Predictions;
using CC_Plugin.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{
    internal static class UpdateOccupantLoadFactors
    {
        public static void UpdateLoadFactor(this Document doc)
        {
            try
            {
                var Rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToElements().ToList();
                foreach (var r in Rooms) r.UpdateLoadFactor();
            }
            catch (Exception e) { e.OutputError(); }
        }
        public static void UpdateLoadFactor(this Element ele)
        {
            try
            {
                Room r = ele as Room;
                var name = r.Name;
                var pred = OLFNetwork.Predict(name, CMDLibrary.WriteNull);
                var numb = pred.ToList().IndexOf(pred.Max());
                var loadfactor = Enum.GetNames(typeof(OccLoadFactor)).ToList()[numb].Remove(0, 1);
                r.SetIntParam(
                    Params.OccupantLoadFactor,
                    loadfactor);
            }
            catch (Exception e) { e.OutputError(); }
        }
    }
}
