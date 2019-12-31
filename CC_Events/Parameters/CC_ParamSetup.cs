using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

using System.IO;
using System.Reflection;

namespace CC_Plugin
{
    public class AddParam
    {
        Document doc { get; }
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static void Run(Definition def, Document doc, Param p)
        {
            if (doc.IsFamilyDocument)
            {
                try
                {
                    doc.FamilyManager.AddParameter(def as ExternalDefinition, p.BuiltInGroup, p.Inst);
                }
                catch
                { }
            }
            else
            {
                try
                {
                    if (!doc.ParameterBindings.Contains(def))
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
                }
                catch
                { }
            }
        }
        public static Definition SetupParam(DefinitionFile df, Document doc, Param p)
        {
            if (df.Groups.get_Item(p.ParamGroup) == null)
            {
                DefinitionGroup group = df.Groups.Create(p.ParamGroup);
                return group.Definitions.Create(new ExDefOptions(p).opt);
            }
            else
            {
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
        public static ExternalDefinition SetupExtParam(DefinitionFile df, Document doc, Param p)
        {
            if (df.Groups.get_Item(p.ParamGroup) == null)
            {
                DefinitionGroup group = df.Groups.Create(p.ParamGroup);
                return group.Definitions.Create(new ExDefOptions(p).opt) as ExternalDefinition;
            }
            else
            {
                DefinitionGroup group = df.Groups.get_Item(p.ParamGroup);
                if (df.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
                {
                    return group.Definitions.Create(new ExDefOptions(p).opt) as ExternalDefinition;
                }
                else
                {
                    return group.Definitions.get_Item(p.name) as ExternalDefinition;
                }
            }
        }
    }
}