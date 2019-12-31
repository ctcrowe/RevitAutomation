using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Reflection;

namespace CC_Plugin
{

    internal class FamilyData
    {
        private static List<Param> Parameters = new List<Param>() { ParameterLibrary.FDataGeom };
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static void CollectData(Document doc)
        {
            List<ElementId> Masses = new FilteredElementCollector(doc).OfClass(typeof(GeometryElement)).ToElementIds().ToList();
            
            FamilyParameter param = doc.FamilyManager.get_Parameter(ParameterLibrary.FDataGeom.ID);
            foreach (FamilyType type in doc.FamilyManager.Types)
            {
                doc.FamilyManager.CurrentType = type;
                doc.FamilyManager.Set(param, Masses.Count());
            }
            WriteParamToTTD.Run(doc, "FData", Parameters);
        }
    }
}
