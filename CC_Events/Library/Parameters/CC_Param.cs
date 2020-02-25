using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;

namespace CC_Plugin
{
    internal static class RevitParams
    {
        public static void AddFamilyParam(this Param p, Document doc)
        {
            if(doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, p.BuiltInGroup, p.Inst);
                }
            }
        }
        public static void AddProjectParam(this Param p, Document doc)
        {
            if(!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                if(!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        foreach(BuiltInCategory cat in p.Categories)
                        {
                            if(!Set.Contains(Category.GetCategory(doc, cat)))
                                set.Insert(Category.GetCategory(doc, cat));
                        }
                        if (set.Size > 0)
                        {
                            if (p.Inst)
                            {
                                InstanceBinding binding = new InstanceBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                            else
                            {
                                TypeBinding binding = new TypeBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        public static void AddSpaceParam(this Param p, Document doc)
        {
            if(doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, p.BuiltInGroup, p.Inst);
                }
            }
        }
        private static Definition CreateDefinition(this Param p, Document doc)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(p.ParamGroup) == null)
            {
                DefinitionGroup newgroup = df.Groups.Create(p.ParamGroup);
                if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
                {
                    return newgroup.Definitions.Create(new ExDefOptions(p).opt);
                }
                else
                {
                    return newgroup.Definitions.get_Item(p.name);
                }
            }
            DefinitionGroup group = df.Groups.get_Item(p.ParamGroup);
            if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
            {
                return group.Definitions.Create(new ExDefOptions(p).opt);
            }
            else
            {
                return group.Definitions.get_Item(p.name);
            }
        }
    }
    internal class RevitParamEdits
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";
        
        public static void Add_FamilyParam(Document doc, Param p)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile DefFile = app.OpenSharedParameterFile();
            if (doc.IsFamilyDocument)
            {
                ExternalDefinition def = SetupParamDefinition(doc, p) as ExternalDefinition;
                if (doc.FamilyManager.get_Parameter(p.ID) == null)
                    doc.FamilyManager.AddParameter(def, p.BuiltInGroup, p.Inst);
            }
        }
        public static void Add_ProjectInfoParam(Document doc, Param p)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile DefFile = app.OpenSharedParameterFile();
            
            if(!doc.IsFamilyDocument)
            {
                Definition def = SetupParamDefinition(doc, p);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        foreach (BuiltInCategory cat in p.Categories)
                        {
                            if (!set.Contains(Category.GetCategory(doc, cat)))
                                set.Insert(Category.GetCategory(doc, cat));
                        }
                        if (set.Size > 0)
                        {
                            if (p.Inst)
                            {
                                InstanceBinding binding = new InstanceBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                            else
                            {
                                TypeBinding binding = new TypeBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        public static bool Get_BooleanFamilyParam(Document doc, Param p)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(p.ID);
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        int i = t.AsInteger(param) ?? 0;
                        if(i == 1)
                            return true;
                    }
                }
            }
            return false;
        }
        private static Definition SetupParamDefinition(Document doc, Param p)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(p.ParamGroup) == null)
            {
                DefinitionGroup newgroup = df.Groups.Create(p.ParamGroup);
                if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
                {
                    return newgroup.Definitions.Create(new ExDefOptions(p).opt);
                }
                else
                {
                    return newgroup.Definitions.get_Item(p.name);
                }
            }
            DefinitionGroup group = df.Groups.get_Item(p.ParamGroup);
            if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
            {
                return group.Definitions.Create(new ExDefOptions(p).opt);
            }
            else
            {
                return group.Definitions.get_Item(p.name);
            }
        }
    }
    
    public class Param
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public string ParamGroup { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public BuiltInCategory[] Categories { get; set; }
        public BuiltInParameterGroup BuiltInGroup { get; set; }
        public Guid ID { get; set; }
        public Boolean Vis { get; set; }
        public string Description { get; set; }
        public Boolean UsrMod { get; set; }
        public Boolean Inst { get; set; }
        public Boolean Fixed { get; set; }
        public Param(string name, int type, Guid id, string paramgroup, BuiltInCategory[] categories,
            BuiltInParameterGroup bpg, string description, bool vis, bool inst, bool usrmod, bool fix)
        {
            this.name = name;
            this.type = type;
            this.ID = id;
            this.ParamGroup = paramgroup;
            this.Categories = categories;
            this.BuiltInGroup = bpg;
            this.Description = description;
            this.Vis = vis;
            this.Inst = inst;
            this.UsrMod = usrmod;
            this.Fixed = fix;
        }
        public void Add(Document doc)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile DefFile = app.OpenSharedParameterFile();
            if (doc.IsFamilyDocument)
            {
                ExternalDefinition def = SetupParam(doc) as ExternalDefinition;
                if (doc.FamilyManager.get_Parameter(ID) == null)
                    doc.FamilyManager.AddParameter(def, BuiltInGroup, Inst);
            }
            else
            {
                Definition def = SetupParam(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        foreach (BuiltInCategory cat in Categories)
                        {
                            if (!set.Contains(Category.GetCategory(doc, cat)))
                                set.Insert(Category.GetCategory(doc, cat));
                        }
                        if (set.Size > 0)
                        {
                            if (Inst)
                            {
                                InstanceBinding binding = new InstanceBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                            else
                            {
                                TypeBinding binding = new TypeBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        public string Set(Document doc, string value)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(ID) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(ID);
                    if (doc.FamilyManager.Types.Size < 1)
                        doc.FamilyManager.NewType("Automatic Type");
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        doc.FamilyManager.Set(param, value);
                    }
                    return value;
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(ID) != null)
                {
                    doc.ProjectInformation.get_Parameter(ID).Set(value);
                    return value;
                }
            }
            return null;
        }
        public string Set(FamilyInstance inst, string value)
        {
            if (Inst)
            {
                if (inst.get_Parameter(ID) != null)
                {
                    inst.get_Parameter(ID).Set(value);
                    return value;
                }
            }
            else
            {
                FamilySymbol symb = inst.Symbol;
                if (symb.get_Parameter(ID) != null)
                {
                    symb.get_Parameter(ID).Set(value);
                    return value;
                }
            }
            return null;
        }
        public string Set(Room r, string value)
        {
            if (r.get_Parameter(ID) != null)
            {
                r.get_Parameter(ID).Set(value);
                return value;
            }
            else
                return null;
        }
        public string Set(Document doc, double value)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(ID) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(ID);
                    if (doc.FamilyManager.Types.Size < 1)
                        doc.FamilyManager.NewType("Automatic Type");
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        doc.FamilyManager.Set(param, value);
                    }
                    return value.ToString();
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(ID) != null)
                {
                    doc.ProjectInformation.get_Parameter(ID).Set(value);
                    return value.ToString();
                }
            }
            return null;
        }
        public string Set(FamilyInstance inst, double value)
        {
            if (Inst)
            {
                if (inst.get_Parameter(ID) != null)
                {
                    inst.get_Parameter(ID).Set(value);
                    return value.ToString();
                }
            }
            else
            {
                FamilySymbol symb = inst.Symbol;
                if (symb.get_Parameter(ID) != null)
                {
                    symb.get_Parameter(ID).Set(value);
                    return value.ToString();
                }
            }
            return null;
        }
        public string Set(Room r, double value)
        {
            if (r.get_Parameter(ID) != null)
            {
                r.get_Parameter(ID).Set(value);
                return value.ToString();
            }
            else
                return null;
        }
        public string Set(Document doc, int value)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(ID) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(ID);
                    if (doc.FamilyManager.Types.Size < 1)
                        doc.FamilyManager.NewType("Automatic Type");
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        doc.FamilyManager.Set(param, value);
                    }
                    return value.ToString();
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(ID) != null)
                {
                    doc.ProjectInformation.get_Parameter(ID).Set(value);
                    return value.ToString();
                }
            }
            return null;
        }
        public string Set(FamilyInstance inst, int value)
        {
            if (Inst)
            {
                if (inst.get_Parameter(ID) != null)
                {
                    inst.get_Parameter(ID).Set(value);
                    return value.ToString();
                }
            }
            else
            {
                FamilySymbol symb = inst.Symbol;
                if (symb.get_Parameter(ID) != null)
                {
                    symb.get_Parameter(ID).Set(value);
                    return value.ToString();
                }
            }
            return null;
        }
        public string Set(Room r, int value)
        {
            if (r.get_Parameter(ID) != null)
            {
                r.get_Parameter(ID).Set(value);
                return value.ToString();
            }
            else
                return null;
        }
        public string Get(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(ID) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(ID);
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        if (t.HasValue(param) && !String.IsNullOrEmpty(t.AsString(param)))
                            return t.AsString(param);
                    }
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(ID) != null)
                {
                    return doc.ProjectInformation.get_Parameter(ID).AsString();
                }
            }
            return null;
        }
        public string Get(FamilyInstance inst)
        {
            if (Inst)
            {
                if (inst.get_Parameter(ID) != null)
                {
                    return inst.get_Parameter(ID).AsString();
                }
            }
            else
            {
                FamilySymbol symb = inst.Symbol;
                if (symb.get_Parameter(ID) != null)
                {
                    return symb.get_Parameter(ID).AsString();
                }
            }
            return null;
        }
        public string Get(Element e)
        {
            if (Inst)
            {
                if (e.get_Parameter(ID) != null)
                {
                    return e.get_Parameter(ID).AsString();
                }
            }
            else
            {
                if (e.get_Parameter(ID) != null)
                {
                    return e.get_Parameter(ID).AsString();
                }
            }
            return null;
        }
        public string Get(Family F)
        {
            ElementId eid = F.GetFamilySymbolIds().FirstOrDefault();
            Element e = F.Document.GetElement(eid);
            if(e.get_Parameter(ID) != null)
            {
                return e.get_Parameter(ID).AsString();
            }
            return null;
        }
        public string Get(Room r)
        {
            if (r.get_Parameter(ID) != null)
                return r.get_Parameter(ID).AsString();
            return null;
        }
        private Definition SetupParam(Document doc)
        {
            Application app = doc.Application;
            app.SharedParametersFilename = SharedParams;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(ParamGroup) == null)
            {
                DefinitionGroup group = df.Groups.Create(ParamGroup);
                return group.Definitions.Create(new ExDefOptions(this).opt);
            }
            else
            {
                DefinitionGroup group = df.Groups.get_Item(ParamGroup);
                if (df.Groups.get_Item(ParamGroup).Definitions.get_Item(name) == null)
                {
                    return group.Definitions.Create(new ExDefOptions(this).opt);
                }
                else
                {
                    return group.Definitions.get_Item(name);
                }
            }
        }
    }
    public static class ParamType
    {
        public const int Bool = 0;
        public const int Int = 1;
        public const int Double = 2;
        public const int Length = 3;
        public const int String = 4;
        public const int Material = 5;
        public const int Area = 6;
    }
}
