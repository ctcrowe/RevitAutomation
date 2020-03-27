using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_Library.Parameters;

namespace CC_Plugin
{
    public static class SetParameters
    {
        public static bool CheckID(this Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(CCParameter.CC_ID.GetGUID()) != null)
                {
                    FamilyParameter p = doc.FamilyManager.get_Parameter(CCParameter.CC_ID.GetGUID());
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        string v = t.AsString(p);
                        if(string.IsNullOrEmpty(v) || string.IsNullOrWhiteSpace(v) || v.Length < 10)
                            return true;
                    }
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(CCParameter.CC_ID.GetGUID()) != null)
                {
                    Parameter p = doc.ProjectInformation.get_Parameter(CCParameter.CC_ID.GetGUID());
                    string v = p.AsString();
                    if (string.IsNullOrEmpty(v) || string.IsNullOrWhiteSpace(v) || v.Length < 10)
                        return true;
                }
            }
            return false;
        }
        public static void SetID(this Document doc, bool check)
        {
            if (check)
            {
                string ID = Guid.NewGuid().ToString("N");
                if (doc.IsFamilyDocument)
                {
                    if(doc.FamilyManager.get_Parameter(CCParameter.CC_ID.GetGUID()) != null)
                    {
                        FamilyParameter p = doc.FamilyManager.GetParameters().Where(x => x.Definition.Name == CCParameter.CC_ID.ToString()).First();
                        foreach (FamilyType t in doc.FamilyManager.Types)
                        {
                            doc.FamilyManager.CurrentType = t;
                            doc.FamilyManager.Set(p, ID);
                        }
                    }
                }
                else
                {
                    if (doc.ProjectInformation.get_Parameter(CCParameter.CC_ID.GetGUID()) != null)
                    {
                        Parameter p = doc.ProjectInformation.LookupParameter(CCParameter.CC_ID.ToString());
                        p.Set(ID);
                    }
                }
                TaskDialog.Show("NEW ID", ID);
            }
        }
        public static void SetFamilyParam(this Document doc, CCParameter p, string value)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.GetGUID()) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(p.GetGUID());
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        doc.FamilyManager.Set(param, value);
                    }
                }
            }
        }
        public static void SetProjectParam(this Document doc, CCParameter p, string value)
        {
            if (!doc.IsFamilyDocument)
                if (doc.ProjectInformation.get_Parameter(p.GetGUID()) != null)
                    doc.ProjectInformation.get_Parameter(p.GetGUID()).Set(value);
        }
        public static void SetRoomParam(this Element e, CCParameter p, string value)
        {
            if (e.get_Parameter(p.GetGUID()) != null)
                e.get_Parameter(p.GetGUID()).Set(value);
        }
    }
}