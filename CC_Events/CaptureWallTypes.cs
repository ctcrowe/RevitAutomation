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
    public static class CaptureWallTypes
    {
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string dir = directory + "\\CC_ElesByID";

        public static void CatpureWalls(this Document doc)
        {
            List<Element> WallCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).ToList();
            string filename = directory + "\\FOUNDLABELS_StudLayer.csv";
            List<string> lines = new List<string>();

            foreach (Element e in WallCollector)
            {
                Wall w = e as Wall;
                if (w != null)
                {
                    TaskDialog.Show("Test", "Wall Found");
                    WallType wt = w.WallType;
                    if (wt.Kind == WallKind.Basic)
                    {
                        TaskDialog.Show("Test", "Wall Kind is Basic");
                        var layers = wt.GetCompoundStructure().GetLayers();

                        string s = "";
                        foreach (var layer in layers)
                        {
                            var materialId = layer.MaterialId;
                            if (materialId != ElementId.InvalidElementId)
                            {
                                Material mat = doc.GetElement(layer.MaterialId) as Material;
                                s += mat.Name + ',';
                            }
                            else
                            {
                                s += "NullMaterial,";
                            }
                        }
                        lines.Add(s);
                    }
                }
            }

            File.WriteAllLines(filename, lines);
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CollectWalls : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            doc.CatpureWalls();
            return Result.Succeeded;
        }
    }
}
