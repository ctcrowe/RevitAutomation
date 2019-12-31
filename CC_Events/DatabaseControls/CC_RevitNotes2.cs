using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

using System.Collections.Generic;
using System.Linq;
using System.IO;

using CC_CoreData;

namespace CC_Events
{
    class Notes
    {
        public static void EnterNotes(Document currentDoc)
        {
            ProjectInfo info = currentDoc.ProjectInformation;

            if (!File.Exists(DBIdentifiers.SharedParams))
            {
                using (FileStream stream = File.Create(DBIdentifiers.SharedParams))
                {
                    stream.Close();
                }
            }
            Application App = currentDoc.Application;
            App.SharedParametersFilename = DBIdentifiers.SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();
            //ParamConstruction.ParameterGroupBindingSetup(deffile, currentDoc, ParameterGroup.ProjectAnalysisParams);

            string filename = RevitDatabase.GetFileName();
            //Variable[] vars = Variable.ReadVarFile(filename);

            List<Element> Doors = Variables.getDoors(currentDoc);
            List<Element> Users = Variables.getUsers(currentDoc);
            List<Element> Equip = Variables.getEquipment(currentDoc);
            List<Element> Mats = Variables.getMaterials(currentDoc);

            /*for (int i = 0; i < vars.Count(); i++)
            {
                Variables.EnterVar(Variables.PickElementSet(Mats, Equip, Users, Doors, vars[i]), vars[i]);
            }*/
        }
    }
}