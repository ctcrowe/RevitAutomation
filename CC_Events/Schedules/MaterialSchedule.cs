using System.Collections.Generic;
using System.Linq;
using System.IO;

using CC_Library.Parameters;
using CC_Plugin.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace CC_Plugin.Schedules
{
    public class MaterialSchedule
    {
        private static List<CC_Library.Parameters.Param> MatPars = new List<CC_Library.Parameters.Param>()
        {
            MaterialParams.Category,
            MaterialParams.Manufacturer,
            MaterialParams.Model,
            MaterialParams.ColorFinish,
            MaterialParams.PatternStyle,
            MaterialParams.FilterFlags
        };

        public static void CreateSched(Document doc)
        {
            var schedCollector = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).ToList();
            if (!schedCollector.Any(x => x.Name == "FINISH LEGEND"))
            {
                using (Transaction t = new Transaction(doc, "Add Material Schedule"))
                {
                    t.Start();
                    var sched = ViewSchedule.CreateMaterialTakeoff(doc, ElementId.InvalidElementId);
                    sched.Name = "FINISH LEGEND";

                    var Mark = sched.Definition.GetSchedulableFields()
                        .Where(x => x.FieldType == ScheduleFieldType.Material)
                        .FirstOrDefault(x => (BuiltInParameter)x.ParameterId.IntegerValue == BuiltInParameter.ALL_MODEL_MARK);
                    if (Mark != null)
                        sched.Definition.AddField(Mark);

                    foreach (var sd in MatPars)
                    {
                        var Field = sched.Definition.GetSchedulableFields().
                            Where(x => x.FieldType == ScheduleFieldType.Material).
                            FirstOrDefault(x => IsSharedParameterSchedulableField(doc, x.ParameterId, sd));

                        if(Field != null)
                            sched.Definition.AddField(Field);
                    }

                    t.Commit();
                }
            }
            else
                TaskDialog.Show("Error", "The Schedule Already Exists");
        }
        private static bool IsSharedParameterSchedulableField(Document document, ElementId parameterId, CC_Library.Parameters.Param par)
        {
            var sharedParameterElement = document.GetElement(parameterId) as SharedParameterElement;
            return sharedParameterElement?.GuidValue == par.Guid;
        }
        public static void Export(Document doc, string filename)
        {
            List<string> Lines = new List<string>();
            Lines.Add("MATERIAL SCHEDULE\tMARK\tCATEGORY\tMANUFACTURER\tMODEL\tCOLOR FINISH\tPATTERN STYLE\tFILTER FLAGS");
            File.WriteAllLines(filename, Lines);

            var MatCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).ToList();
            foreach (var e in MatCollector)
            {
                string line = "";
                try { line += "\t" + e.Name; } catch { line += "\t"; }
                foreach (var par in MatPars)
                {
                    try { line += "\t" + e.GetElementParam(par); } catch { line += "\t"; }
                }
                Lines.Add(line);
            }
            File.WriteAllLines(filename, Lines);
        }
        private static List<Element> GetGenericModels(Document doc, Family fam)
        {
            List<Element> GenMods = new List<Element>();
            var collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).ToList();
            foreach(var c in collector)
            {
                FamilyInstance f = c as FamilyInstance;
                if(f != null)
                    if (f.Symbol.Family.Name == fam.Name)
                        GenMods.Add(c);
            }
            return GenMods;
        }
        public static void Import(Document doc, string filename)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Add Materials"))
            {
                Family fam;
                tg.Start();
                using (Transaction t = new Transaction(doc, "Load Family"))
                {
                    t.Start();
                    fam = EmbeddedFamilies.run(doc, "MaterialInstance");
                    t.Commit();
                }
                FamilySymbol symbol = doc.GetElement(fam.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
                if (!symbol.IsActive)
                {
                    using (Transaction t = new Transaction(doc, "Activate Symbol"))
                    {
                        t.Start();
                        symbol.Activate(); doc.Regenerate();
                        t.Commit();
                    }
                }
                var Lines = File.ReadAllLines(filename);
                var Mats = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).ToList();
                var GenMods = GetGenericModels(doc, fam);
                foreach (var line in Lines)
                {
                    if (Mats.Any(x => x.Name == line.Split('\t')[1]))
                    {
                        var m = Mats.FirstOrDefault(x => x.Name == line.Split('\t')[1]);
                        if (m != null)
                        {
                            using (Transaction t = new Transaction(doc, "Update Material : " + m.Name))
                            {
                                t.Start();
                                var e = m as Element;
                                for (int i = 0; i < MatPars.Count(); i++)
                                    try { e.SetElementParam(MatPars[i], line.Split('\t')[i + 2]); } catch { }
                                t.Commit();
                            }
                        }
                        if (!GenMods.Any(x => x.GetElementParam(SpecialParams.MaterialInstance) == line.Split('\t')[1]))
                        {
                            using (Transaction t = new Transaction(doc, "Place Material : " + line.Split('\t')[1]))
                            {
                                t.Start();
                                FamilyInstance f = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), doc.GetElement(fam.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol, StructuralType.NonStructural);
                                f.SetElementParam(SpecialParams.MaterialInstance, line.Split('\t')[1]);
                                t.Commit();
                            }
                        }
                    }
                    else
                    {
                        using (Transaction t = new Transaction(doc, "Create Material : " + line.Split('\t')[1]))
                        {
                            t.Start();
                            ElementId matID = Material.Create(doc, line.Split('\t')[1]);
                            Element mat = doc.GetElement(matID);
                            for (int i = 0; i < MatPars.Count(); i++)
                                try { mat.SetElementParam(MatPars[i], line.Split('\t')[i + 2]); } catch { }
                            doc.Regenerate();
                            t.Commit();
                        }
                        using (Transaction t = new Transaction(doc, "Place Material : " + line.Split('\t')[1]))
                        {
                            t.Start();
                            FamilyInstance f = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), doc.GetElement(fam.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol, StructuralType.NonStructural);
                            f.SetElementParam(SpecialParams.MaterialInstance, line.Split('\t')[1]);
                            t.Commit();
                        }
                    }
                }
                if (GenMods.Any())
                {
                    List<ElementId> del = new List<ElementId>();
                    foreach (var g in GenMods)
                        if (!Lines.Any(x => x.Split('\t')[1] == g.GetElementParam(SpecialParams.MaterialInstance)))
                            del.Add(g.Id);
                    using (Transaction t = new Transaction(doc, "Delete Materials"))
                    {
                        t.Start();
                        doc.Delete(del);
                        t.Commit();
                    }
                }
                tg.Commit();
            }
        }
    }
}