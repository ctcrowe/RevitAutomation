using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;

using CC_Plugin.Parameters;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class ProjectData
    {
        private static List<CC_Library.Parameters.Param> Parameters = new List<CC_Library.Parameters.Param>(){ ProjectParams.Families, ProjectParams.FamilyInstances, ProjectParams.Materials, ProjectParams.ModelLines,
            ProjectParams.RefPlanes, ProjectParams.Sheets, ProjectParams.Views, ProjectParams.ViwesOnSheets };
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static void CollectData(this Document doc)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Output Xml and Update Data"))
            {
                tg.Start();
                using (Transaction t = new Transaction(doc, "Add Parameters:"))
                {
                    t.Start();
                    foreach (Param p in Parameters)
                    {
                        doc.AddParam(p);
                    }
                    t.Commit();
                }
                using (Transaction t = new Transaction(doc, "Collect Project Data"))
                {
                    t.Start();
                    var fams = new FilteredElementCollector(doc).OfClass(typeof(Family)).Count();
                    var insts = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).Count();
                    var mats = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).Count();
                    var Sheets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).ToElementIds().ToList();
                    var Views = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).ToElementIds().ToList();
                    var vos = Views.Where(x => !Viewport.CanAddViewToSheet(doc, Sheets.FirstOrDefault(), x)).Count();
                    doc.SetParam(ProjectParams.Families, fams.ToString());
                    doc.SetParam(ProjectParams.FamilyInstances, insts.ToString());
                    doc.SetParam(ProjectParams.Materials, mats.ToString());
                    doc.SetParam(ProjectParams.Sheets, Sheets.Count().ToString());
                    doc.SetParam(ProjectParams.Views, Views.Count().ToString());
                    doc.SetParam(ProjectParams.ViwesOnSheets, vos.ToString());
                    t.Commit();
                }
                tg.Commit();
            }
        }
    }
}