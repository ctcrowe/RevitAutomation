
using Autodesk.Revit.DB;

using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class GetParameters
    {
        public static string GetDocumentParam(this Document doc, CCParameter p)
        {
            if (doc.IsFamilyDocument)
                return doc.GetFamilyParam(p);
            else
                return doc.GetProjectInfoParam(p);
            return null;
        }
        public static string GetElementParam(this Element e, CCParameter p)
        {
            if (e.get_Parameter(p.GetGUID()) != null)
            {
                return e.get_Parameter(p.GetGUID()).GetParameterValue();
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
        private static string GetProjectInfoParam(this Document doc, CCParameter p)
        {
            if (!doc.IsFamilyDocument)
            {
                if (doc.ProjectInformation.get_Parameter(p.GetGUID()) != null)
                {
                    return doc.ProjectInformation.get_Parameter(p.GetGUID()).GetParameterValue();
                }
            }
            return null;
        }
        private static string GetFamilyParam(this Document doc, CCParameter p)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.GetGUID()) != null)
                {
                    FamilyParameter par = doc.FamilyManager.get_Parameter(p.GetGUID());
                    foreach(FamilyType t in doc.FamilyManager.Types)
                    {
                        if(!string.IsNullOrEmpty(t.GetParameterValue(par)))
                            return t.GetParameterValue(par);
                    }
                }
            }
            return null;
        }
    }
}