using Autodesk.Revit.DB;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class GetRevitParams
    {
        public static void GetIDParam(this IDParam p, Document doc)
        {
            if (doc.IsFamilyDocument)
                p.Value = p.GetFamilyParam(doc);
            else
                p.Value = p.GetProjectParam(doc);
        }
        public static string GetEleParam<t>(this t p, Element ele) where t : Param
        {
            return ele.get_Parameter(p.ID).AsValueString();
        }
        public static string GetFamilyParam<t>(this t p, Document doc) where t : Param
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) != null)
                {
                    FamilyType type = doc.FamilyManager.CurrentType;
                    FamilyParameter par = doc.FamilyManager.get_Parameter(p.ID);
                    return type.AsValueString(par);
                }
            }
            return null;
        }
        public static string GetProjectParam<t>(this t p, Document doc) where t : Param
        {
            if (!doc.IsFamilyDocument)
            {
                return doc.ProjectInformation.get_Parameter(p.ID).AsValueString();
            }
            return null;
        }
    }
}