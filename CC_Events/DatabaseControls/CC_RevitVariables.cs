using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

using System.Collections.Generic;
using System.Linq;
using System.IO;

using CC_CoreData;

namespace CC_Events
{
    class Variables
    {
        public static List<Element> PickElementSet(List<Element> Mats, List<Element> Equip,
            List<Element> Fac, List<Element> Door, Variable var)
        {
            switch(var.Category)
            {
                default:
                case ParamCategory.Materials:
                    return Mats;
                case ParamCategory.Equipment:
                    return Equip;
                case ParamCategory.Users:
                    return Fac;
                case ParamCategory.Doors:
                    return Door;
            }
        }
        public static string GetFile(Document currentDoc, Variable var)
        {
            ProjectInfo info = currentDoc.ProjectInformation;
            switch(var.Category)
            {
                default:
                case ParamCategory.Materials:
                    return info.get_Parameter(ParameterLibrary.MaterialFile.ID).AsString();
                case ParamCategory.Equipment:
                    return info.get_Parameter(ParameterLibrary.EquipFile.ID).AsString();
                case ParamCategory.Doors:
                    return info.get_Parameter(ParameterLibrary.DoorFile.ID).AsString();
                case ParamCategory.Users:
                    return info.get_Parameter(ParameterLibrary.StaffFile.ID).AsString();
            }
        }
        public static List<Element> getMaterials(Document currentDoc)
        {
            ProjectInfo info = currentDoc.ProjectInformation;
            if(!File.Exists(DBIdentifiers.SharedParams))
            {
                using (FileStream stream = File.Create(DBIdentifiers.SharedParams))
                {
                    stream.Close();
                }
            }
            Application App = currentDoc.Application;
            App.SharedParametersFilename = DBIdentifiers.SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();

            FilteredElementCollector matCollector = new FilteredElementCollector(currentDoc);
            return matCollector.OfClass(typeof(Material)).ToElements().ToList();
        }
        public static List<Element> getEquipment(Document currentDoc)
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

            List<ElementId> TypeIds = new List<ElementId>();
            List<Element> Eles = new List<Element>();
            FilteredElementCollector eleCollector = new FilteredElementCollector(currentDoc);
            ICollection<Element> eles = eleCollector.OfClass(typeof(FamilySymbol)).ToList();
            foreach (Element symb in eles)
            {
                if (symb.LookupParameter(ParameterLibrary.ToFile.name) != null && !TypeIds.Contains(symb.Id))
                {
                    if (symb.Category == Category.GetCategory(currentDoc, BuiltInCategory.OST_GenericModel) ||
                        symb.Category == Category.GetCategory(currentDoc, BuiltInCategory.OST_Furniture) ||
                        symb.Category == Category.GetCategory(currentDoc, BuiltInCategory.OST_FurnitureSystems) ||
                        symb.Category == Category.GetCategory(currentDoc, BuiltInCategory.OST_SpecialityEquipment) ||
                        symb.Category == Category.GetCategory(currentDoc, BuiltInCategory.OST_PlumbingFixtures))
                    {
                        TypeIds.Add(symb.Id);
                        Eles.Add(symb);
                    }
                }
            }
            return Eles;
        }
        public static List<Element> getDoors(Document currentDoc)
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

            List<ElementId> TypeIds = new List<ElementId>();
            List<Element> Eles = new List<Element>();
            FilteredElementCollector eleCollector = new FilteredElementCollector(currentDoc);
            ICollection<Element> eles = eleCollector.OfClass(typeof(FamilySymbol)).ToList();
            foreach (Element symb in eles)
            {
                if (symb.LookupParameter(ParameterLibrary.ToFile.name) != null && !TypeIds.Contains(symb.Id))
                {
                    if (symb.Category == Category.GetCategory(currentDoc, BuiltInCategory.OST_Doors))
                    {
                        TypeIds.Add(symb.Id);
                        Eles.Add(symb);
                    }
                }
            }
            return Eles;
        }
        public static List<Element> getUsers(Document currentDoc)
        {
            ViewSchedule sched = KeySchedule.GetKeySchedule(currentDoc, RevitFacilities.ViewNames);
            IEnumerable<Element> eles = from e in new FilteredElementCollector(sched.Document, sched.Id) select e;
            return eles.ToList();
        }
        public static void EnterVar(List<Element> Elements, Variable var)
        {
            Document currentDoc = Elements[0].Document;
            if(Elements.Any(x => x.get_Parameter(ParameterLibrary.Identifier.ID).AsString() == var.Element))
            {
                Element e = Elements.Where(x => x.get_Parameter(ParameterLibrary.Identifier.ID).AsString() == var.Element).First();
                Parameter p = e.get_Parameter(var.Parameter.Param.ID);
                p.Set(var.Parameter.Value);
            }
            string file = GetFile(currentDoc, var);
            if(file != null || file != string.Empty)
            {
                DatabaseIO.WriteVarFile(file, var);
            }
        }
    }
}