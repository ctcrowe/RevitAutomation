using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;

namespace CC_Plugin
{
    internal class ProjectData
    {
        private static List<Param> Parameters = new List<Param>(){ ParameterLibrary.PDataFamilies, ParameterLibrary.PDataFamilyInstances,
            ParameterLibrary.PDataMaterials, ParameterLibrary.PDataRefPlanes, ParameterLibrary.PDataSheets,
            ParameterLibrary.PDataViews, ParameterLibrary.PDataVoS };
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static void CollectData(Document doc, XmlLib lib)
        {
            XElement xdoc = lib.GetElement(IDParam.Get(doc), "PROJECT");
            try
            {
                foreach (Param p in Parameters)
                {
                    p.Add(doc);
                }
            }
            catch { }
            try
            {
                XElement Dataset = new XElement("PDATA");
                Dataset.Add(new XAttribute("TIME", DateTime.Now.ToString("yyyyMMddhhmmss")));
                Dataset.Add(new XAttribute("FAMILIES", ParameterLibrary.PDataFamilies.Set(doc,
                    new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElementIds().ToList().Count())));
                Dataset.Add(new XAttribute("INSTANCES", ParameterLibrary.PDataFamilyInstances.Set(doc,
                    new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).ToElementIds().ToList().Count())));
                Dataset.Add(new XAttribute("REF PLANES", ParameterLibrary.PDataRefPlanes.Set(doc,
                    new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CLines).ToElementIds().ToList().Count())));
                Dataset.Add(new XAttribute("MATERIALS", ParameterLibrary.PDataMaterials.Set(doc,
                    new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).ToElementIds().ToList().Count())));

                List<ElementId> SheetCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).ToElementIds().ToList();
                List<ElementId> ViewCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).ToElementIds().ToList();
                Dataset.Add(new XAttribute("VIEWS", ParameterLibrary.PDataViews.Set(doc, ViewCollector.Count())));
                Dataset.Add(new XAttribute("SHEETS", ParameterLibrary.PDataSheets.Set(doc, SheetCollector.Count())));
                Dataset.Add(new XAttribute("VoS", ParameterLibrary.PDataVoS.Set(doc, ViewCollector.
                    Where(x => !Viewport.CanAddViewToSheet(doc, SheetCollector.FirstOrDefault(), x)).Count())));
                xdoc.Add(Dataset);
            }
            catch { }
        }
    }
}