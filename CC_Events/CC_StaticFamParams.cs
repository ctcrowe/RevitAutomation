using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace CC_Plugin
{
    internal class StaticFamilyParams
    {
        private static List<Param> Parameters = new List<Param> {
            ParameterLibrary.OverallDepth, ParameterLibrary.OverallHeight, ParameterLibrary.OverallWidth};
        public static void Execute(Document doc)
        {
            foreach(Param p in Parameters)
            {
                p.Add(doc);
            }
        }
    }
}