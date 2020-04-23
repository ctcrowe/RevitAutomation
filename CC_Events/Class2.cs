using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using CC_Library;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace CC_Plugin
{
    public static class CaptureFamilies
    {
        public static void CaptureFamilySet(this Document doc)
        {
            string filename = doc.ProjectInformation.Name.ToString().GetMyDocs() + ".csv";
            List<string> lines = new List<string>();

            List<Element> instances = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).ToList();
            foreach(Element e in instances)
            {
                FamilyInstance fi = e as FamilyInstance;
                if(fi != null)
                {
                    FamilySymbol fs = fi.Symbol;
                    if(fs != null)
                    {
                        string f = fs.Family.Name;
                        lines.Add(f + ",");
                    }
                }
            }
            File.WriteAllLines(filename, lines);
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CollectFamilies : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            doc.CaptureFamilySet();
            return Result.Succeeded;
        }
    }
}
