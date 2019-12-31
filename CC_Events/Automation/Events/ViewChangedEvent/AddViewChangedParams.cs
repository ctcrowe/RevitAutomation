using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    internal class AddViewChangedParameters
    {
        private static List<Param> GeneralParams = new List<Param>() { ParameterLibrary.FDataGeom };
        private static Param Caseworkparam = ParameterLibrary.FamilyCheck;

        public static Dictionary<Param, Definition> Setup(DefinitionFile deffile, Document doc)
        {
            Dictionary<Param, Definition> defs = new Dictionary<Param, Definition>();
            if (doc.OwnerFamily.FamilyCategory.Name == "Casework"
                    && doc.FamilyManager.get_Parameter(Caseworkparam.ID) == null)
            {
                defs.Add(Caseworkparam, AddParam.SetupParam(deffile, doc, Caseworkparam));
            }
            foreach(Param p in GeneralParams)
            {
                defs.Add(p, AddParam.SetupParam(deffile, doc, p));
            }
            return defs;
        }
        public static void Add(Dictionary<Param, Definition> defs, Document doc)
        {
        }
    }
}
