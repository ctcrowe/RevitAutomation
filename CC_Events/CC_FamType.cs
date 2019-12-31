using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{
    internal class FamType
    {
        public static void Setup(Document doc)
        {
            if (doc.FamilyManager.Types.Size < 1)
                doc.FamilyManager.NewType("Automatic Type");
        }
    }
}