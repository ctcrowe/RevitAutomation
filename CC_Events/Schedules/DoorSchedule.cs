using System.Collections.Generic;
using System.Linq;
using System.IO;

using CC_Library.Parameters;
using CC_Plugin.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin.Schedules
{
    public class DoorSchedule
    {
        private static List<CC_Library.Parameters.Param> DoorParameters = new List<CC_Library.Parameters.Param>()
        {
            DoorParams.PanelHeight,
            DoorParams.PanelWidth,
            DoorParams.PanelMaterial,
            DoorParams.PanelFinish,
            DoorParams.FrameMaterial,
            DoorParams.FrameFinish,
            DoorParams.HeadDetail,
            DoorParams.JambDetail,
            DoorParams.SillDetail,
            DoorParams.HardwareGroup,
            DoorParams.FireRating,
            DoorParams.FilterFlags
        };

        public static void CreateSched(Document doc)
        {
            var schedCollector = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).ToList();
            if (!schedCollector.Any(x => x.Name == "DOOR SCHEDULE"))
            {
                using (Transaction t = new Transaction(doc, "Add Door Schedule"))
                {
                    t.Start();
                    var sched = ViewSchedule.CreateSchedule(doc, Category.GetCategory(doc, BuiltInCategory.OST_Doors).Id);
                    sched.Name = "DOOR SCHEDULE";

                    var Number = sched.Definition.GetSchedulableFields().FirstOrDefault(x => (BuiltInParameter)x.ParameterId.IntegerValue == BuiltInParameter.ALL_MODEL_MARK);
                    if (Number != null)
                        sched.Definition.AddField(Number);

                    var Level = sched.Definition.GetSchedulableFields().FirstOrDefault(x => (BuiltInParameter)x.ParameterId.IntegerValue == BuiltInParameter.SCHEDULE_LEVEL_PARAM);
                    if (Level != null)
                        sched.Definition.AddField(Level);

                    foreach (var sd in DoorParameters)
                    {
                        var Field = sched.Definition.GetSchedulableFields().
                            FirstOrDefault(x => IsSharedParameterSchedulableField(doc, x.ParameterId, sd));

                        if(Field != null)
                        {
                            sched.Definition.AddField(Field);
                        }
                    }

                    var Comments = sched.Definition.GetSchedulableFields().FirstOrDefault(x => (BuiltInParameter)x.ParameterId.IntegerValue == BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    if (Comments != null)
                        sched.Definition.AddField(Comments);

                    t.Commit();
                }
            }
            else
            {
                TaskDialog.Show("Error", "The Schedule Already Exists");
            }
        }
        private static bool IsSharedParameterSchedulableField(Document document, ElementId parameterId, CC_Library.Parameters.Param par)
        {
            var sharedParameterElement = document.GetElement(parameterId) as SharedParameterElement;
            return sharedParameterElement?.GuidValue == par.Guid;
        }
        public static void Export(Document doc, string filename)
        {
            List<string> Lines = new List<string>();
            Lines.Add("DOOR SCHEDULE\tMARK\tLEVEL\tHEIGHT\tWIDTH\tPANEL MATERIAL\tPANEL FINISH\tFRAME MATERIAL\tFRAME FINISH\tHEAD DETAIL\tJAMB DETAIL\tSILL DETAIL\tHARDWARE GROUP\tFIRE RATING\tFILTER FLAGS\tCOMMENTS");
            File.WriteAllLines(filename, Lines);

            var DoorCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors).ToList();
            foreach (var d in DoorCollector)
            {
                string line = "";
                var e = d as Element;
                try { line += "\t" + d.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString(); } catch { line += "\t"; }
                try { line += "\t" + d.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM).AsString(); } catch { line += "\t"; }
                foreach (var par in DoorParameters)
                {
                    try { line += "\t" + e.GetElementParam(par); } catch { line += "\t"; }
                }
                try { line += "\t" + d.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString(); } catch { line += "\t"; }
                Lines.Add(line);
            }
            File.WriteAllLines(filename, Lines);
        }
        public static void Import(Document doc, string filename)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Import Doors"))
            {
                tg.Start();
                var Lines = File.ReadAllLines(filename);
                var Doors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors).ToList();
                foreach (var line in Lines)
                {
                    if (Doors.Any(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == line.Split('\t')[1]))
                    {
                        var d = Doors.FirstOrDefault(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == line.Split('\t')[1]);
                        if (d != null)
                        {
                            using (Transaction t = new Transaction(doc, "Update " + line.Split('\t')[1]))
                            {
                                t.Start();
                                var e = d as Element;
                                for (int i = 0; i < DoorParameters.Count(); i++)
                                    try { e.SetElementParam(DoorParameters[i], line.Split('\t')[i + 3]); } catch { }
                                try { e.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).SetValueString(line.Split('\t').Last()); } catch { }
                                t.Commit();
                            }
                        }
                    }
                }
                tg.Commit();
            }
        }
    }
}