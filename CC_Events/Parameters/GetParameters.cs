using Autodesk.Revit.DB;
using CC_Library.Parameters;

namespace CC_Plugin.Parameters
{
    public static class GetParameters
    {
        public static string GetDocumentParam(this Document doc, Param p)
        {
            if (doc.IsFamilyDocument)
                return doc.GetFamilyParam(p);
            else
                return doc.GetProjectInfoParam(p);
        }
        public static string GetElementTypeParam(this FamilyInstance inst, Param p)
        {
            string paramValue = null;
            FamilySymbol symb = inst.Symbol;
            if (symb.get_Parameter(p.Guid) != null)
            {
                return symb.get_Parameter(p.Guid).GetParameterValue();
            }
            return paramValue;
        }
        public static string GetElementParam(this Element e, Param p)
        {
            if (e.get_Parameter(p.Guid) != null)
            {
                return e.get_Parameter(p.Guid).GetParameterValue();
            }
            return null;
        }
        private static string GetProjectInfoParam(this Document doc, Param p)
        {
            if (!doc.IsFamilyDocument)
            {
                if (doc.ProjectInformation.get_Parameter(p.Guid) != null)
                {
                    return doc.ProjectInformation.get_Parameter(p.Guid).GetParameterValue();
                }
            }
            return null;
        }
        private static string GetFamilyParam(this Document doc, Param p)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.Guid) != null)
                {
                    FamilyParameter par = doc.FamilyManager.get_Parameter(p.Guid);
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        if (!string.IsNullOrEmpty(t.GetParameterValue(par)))
                            return t.GetParameterValue(par);
                    }
                }
            }
            return null;
        }
        private static string GetParameterValue(this Parameter p)
        {
            string paramValue = null;
            switch (p.StorageType)
            {
                case StorageType.Integer:
                    if (ParameterType.YesNo == p.Definition.ParameterType)
                    {
                        if (p.AsInteger() == 1)
                            paramValue = "YES";
                        else
                            paramValue = "NO";
                    }
                    else
                    {
                        paramValue = p.AsValueString();
                    }
                    break;
                case StorageType.Double:
                    paramValue = p.AsValueString();
                    break;
                case StorageType.String:
                    paramValue = p.AsString();
                    break;
                case StorageType.ElementId:
                    try
                    {
                        var ele = p.Element.Document.GetElement(p.AsElementId());
                        paramValue = ele.Name;
                    }
                    catch { }
                    break;
                default:
                    paramValue = null;
                    break;
            }
            return paramValue;
        }
        private static string GetParameterValue(this FamilyType t, FamilyParameter p)
        {
            string paramValue = null;
            switch (p.StorageType)
            {
                case StorageType.Integer:
                    if (ParameterType.YesNo == p.Definition.ParameterType)
                    {
                        if (t.AsInteger(p) == 1)
                            paramValue = "YES";
                        else
                            paramValue = "NO";
                    }
                    else
                    {
                        paramValue = t.AsValueString(p);
                    }
                    break;
                case StorageType.Double:
                    paramValue = t.AsValueString(p);
                    break;
                case StorageType.String:
                    paramValue = t.AsString(p);
                    break;
                default:
                    paramValue = null;
                    break;
            }
            return paramValue;
        }
    }
}