using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin.Schedules
{
    public class DoorSchedule
    {
        /*
         * Parameters
         * Number
         * Level
         * Height
         * Type
         * Panel Height
         * Panel Width
         * Panel Material
         * Panel Finish
         * Frame Material
         * Frame Finish
         * Head Detail
         * Jamb Detail
         * Sill Detail
         * Hardware Group
         * Fire Rating
         * Comments
         */

        public static void CreateSched(Document doc)
        {
            var schedCollector = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).ToList();
            if (!schedCollector.Any(x => x.Name == "DOOR SCHEDULE"))
            {
                using (Transaction t = new Transaction(doc, "Add Door Schedule"))
                {
                    t.Start();
                    var sched = ViewSchedule.CreateSchedule(doc, Category.GetCategory(doc, BuiltInCategory.OST_Doors).Id);
                    t.Commit();
                }
                TaskDialog.Show("Test", "Schedule Created");
            }
            else
            {
                TaskDialog.Show("Error", "The Schedule Already Exists");
            }
        }
    }
}
