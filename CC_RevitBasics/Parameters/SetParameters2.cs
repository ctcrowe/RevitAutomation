using System;
using Autodesk.Revit.DB;
using CC_Library.Parameters;

namespace CC_Plugin
{
    public static class SetParameters
    {
        public static bool CheckStringParam(this Document doc, CCParameter p)
        {
            string v = string.Empty;
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.GetGUID()) != null)
                {
                    FamilyParameter par = doc.FamilyManager.get_Parameter(p.GetGUID());
                    FamilyType t = doc.FamilyManager.CurrentType;
                    try { v = t.AsString(par); } catch { }
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(p.GetGUID()) != null)
                {
                    Parameter par = doc.ProjectInformation.get_Parameter(p.GetGUID());
                    try { v = par.AsString(); } catch { }
                }
            }
            if (v == string.Empty)
                return false;
            if (v == null)
                return false;
            if (v.Length < 30)
                return false;
            return true;
        }
        public static bool CheckID(this Document doc)
        {
            string v = string.Empty;
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(CCParameter.CC_ID.GetGUID()) != null)
                {
                    FamilyParameter p = doc.FamilyManager.get_Parameter(CCParameter.CC_ID.GetGUID());
                    FamilyType t = doc.FamilyManager.CurrentType;
                    try { v = t.AsString(p); } catch { }
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(CCParameter.CC_ID.GetGUID()) != null)
                {
                    Parameter p = doc.ProjectInformation.get_Parameter(CCParameter.CC_ID.GetGUID());
                    try { v = p.AsString(); } catch { }
                }
            }
            if (v == string.Empty)
                return false;
            if (v == null)
                return false;
            if (v.Length < 30)
                return false;
            return true;
        }
        public static void SetID(this Document doc, bool check)
        {
            if (!check)
            {
                string ID = Guid.NewGuid().ToString("N");
                if (doc.IsFamilyDocument)
                {
                    if (doc.FamilyManager.get_Parameter(CCParameter.CC_ID.GetGUID()) != null)
                    {
                        FamilyParameter p = doc.FamilyManager.get_Parameter(CCParameter.CC_ID.GetGUID());
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
                        Parameter p = doc.ProjectInformation.get_Parameter(CCParameter.CC_ID.GetGUID());
                        p.Set(ID);
                    }
                }
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
        public static void SetElementParam(this Element e, CCParameter p, string value)
        {
            if (p < 0)
            {
                e.SetElementInstanceParam(p, value);
            }
            else
            {
                e.SetElementTypeParam(p, value);
            }
        }
        public static void SetProjectParam(this Document doc, CCParameter p, string value)
        {
            if (!doc.IsFamilyDocument)
                if (doc.ProjectInformation.get_Parameter(p.GetGUID()) != null)
                    doc.ProjectInformation.get_Parameter(p.GetGUID()).Set(value);
        }
        public static void SetRoomTextParam(this Element e, CCParameter p, string value)
        {
            if (e.get_Parameter(p.GetGUID()) != null)
                e.get_Parameter(p.GetGUID()).Set(value);
        }
        public static void SetRoomIntParam(this Element e, CCParameter p, string value)
        {
            int i = 0;
            if (int.TryParse(value, out i))
            {
                if (e.get_Parameter(p.GetGUID()) != null)
                    e.get_Parameter(p.GetGUID()).Set(i);
            }
        }
        private static void SetElementInstanceParam(this Element e, CCParameter p, string value)
        {
            if (e.get_Parameter(p.GetGUID()) != null)
                e.get_Parameter(p.GetGUID()).Set(value);
        }
        private static void SetElementTypeParam(this Element e, CCParameter p, string value)
        {
            FamilyInstance i = e as FamilyInstance;
            FamilySymbol s = i.Symbol;
            if (s.get_Parameter(p.GetGUID()) != null)
                s.get_Parameter(p.GetGUID()).Set(value);
        }
    }
}