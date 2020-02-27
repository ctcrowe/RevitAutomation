using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;
using CC_Library;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class GetRevitParams
    {
        
    }
    internal static class SetRevitParams
    {
        public static void SetFamilyParam(this CC_Library.Parameters.Param p, Document doc, string value)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) == null)
                {
                    ExternalDefinition def = p.CreateDefinition(doc) as ExternalDefinition;
                    doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, p.Inst);
                }
            }
        }
        public static void SetProjectParam(this CC_Library.Parameters.Param p, Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_ProjectInformation));
                        InstanceBinding binding = new InstanceBinding(set);
                        doc.ParameterBindings.Insert(def, binding);
                    }
                    catch { }
                }
            }
        }
        public static void SetSpaceParam(this CC_Library.Parameters.Param p, Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Rooms));
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Areas));
                        InstanceBinding binding = new InstanceBinding(set);
                        doc.ParameterBindings.Insert(def, binding);
                    }
                    catch { }
                }
            }
        }
        public static void SetWallParam(this CC_Library.Parameters.Param p, Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                Definition def = p.CreateDefinition(doc);
                if (!doc.ParameterBindings.Contains(def))
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        set.Insert(Category.GetCategory(doc, BuiltInCategory.OST_Walls));
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
                    catch { }
                }
            }
        }
    }
}
