using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using CC_Library.Parameters;

using System.Linq;

namespace CC_Plugin.Parameters
{
    public static class SetParameters
    {
        public static bool CheckStringParam(this Document doc, Param p)
        {
            string v = string.Empty;
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.Guid) != null)
                {
                    FamilyParameter par = doc.FamilyManager.get_Parameter(p.Guid);
                    FamilyType t = doc.FamilyManager.CurrentType;
                    try { v = t.AsString(par); } catch { }
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(p.Guid) != null)
                {
                    Parameter par = doc.ProjectInformation.get_Parameter(p.Guid);
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
                if (doc.FamilyManager.get_Parameter(ProjectParams.CC_ID.Guid) != null)
                {
                    FamilyParameter p = doc.FamilyManager.get_Parameter(ProjectParams.CC_ID.Guid);
                    FamilyType t = doc.FamilyManager.CurrentType;
                    try { v = t.AsString(p); } catch { }
                }
            }
            else
            {
                if (doc.ProjectInformation.get_Parameter(ProjectParams.CC_ID.Guid) != null)
                {
                    Parameter p = doc.ProjectInformation.get_Parameter(ProjectParams.CC_ID.Guid);
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
        public static string SetID(this Document doc)
        {
            if (!doc.CheckID())
            {
                string ID = Guid.NewGuid().ToString("N");
                if (doc.IsFamilyDocument)
                {
                    if (doc.FamilyManager.get_Parameter(ProjectParams.CC_ID.Guid) != null)
                    {
                        FamilyParameter p = doc.FamilyManager.get_Parameter(ProjectParams.CC_ID.Guid);
                        foreach (FamilyType t in doc.FamilyManager.Types)
                        {
                            doc.FamilyManager.CurrentType = t;
                            doc.FamilyManager.Set(p, ID);
                        }
                    }
                }
                else
                {
                    if (doc.ProjectInformation.get_Parameter(ProjectParams.CC_ID.Guid) != null)
                    {
                        Parameter p = doc.ProjectInformation.get_Parameter(ProjectParams.CC_ID.Guid);
                        p.Set(ID);
                    }
                }
                return ID;
            }
            return doc.GetDocumentParam(ProjectParams.CC_ID);
        }
        public static void SetFamilyParam(this Document doc, Param p, string value)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.Guid) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(p.Guid);
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        switch(p.Type)
                        {
                            case ParamType.Text:
                                doc.FamilyManager.Set(param, value);
                                break;
                            case ParamType.Int:
                                doc.FamilyManager.Set(param, int.Parse(value));
                                break;

                        }
                    }
                }
            }
        }
        public static void SetWallTypeParam(this WallType wt, Param p, string value)
        {
            if (wt.get_Parameter(p.Guid) != null)
            {
                Parameter par = wt.get_Parameter(p.Guid);
                switch (par.StorageType)
                {
                    default:
                    case StorageType.String:
                        par.Set(value);
                        break;
                    case StorageType.Double:
                        par.Set(double.Parse(value));
                        break;
                    case StorageType.Integer:
                        par.Set(int.Parse(value));
                        break;
                    case StorageType.ElementId:
                        ElementId mat = wt.Document.GetMaterial(value);
                        if (mat != null && mat != ElementId.InvalidElementId)
                            par.Set(mat);
                        break;
                }
            }
        }
        public static void Set(this Element e, Param p, string value)
        {
            if (p.Instance)
            {
                e.SetElementInstanceParam(p, value);
            }
            else
            {
                e.SetElementTypeParam(p, value);
            }
        }
        public static void SetElementParam(this Element e, Param p, string value)
        {
            if (p.Instance)
            {
                e.SetElementInstanceParam(p, value);
            }
            else
            {
                e.SetElementTypeParam(p, value);
            }
        }
        #region Project
        public static void SetParam(this Document doc, Param p, string value)
        {
            if (!doc.IsFamilyDocument)
                if (doc.ProjectInformation.get_Parameter(p.Guid) != null)
                {
                    Parameter par = doc.ProjectInformation.get_Parameter(p.Guid);
                    switch(par.StorageType)
                    {
                        default:
                        case StorageType.String:
                            par.Set(value);
                            break;
                        case StorageType.Double:
                            par.Set(double.Parse(value));
                            break;
                        case StorageType.Integer:
                            par.Set(int.Parse(value));
                            break;
                    }
                }
        }
        #endregion
        #region Rooms
        public static void SetTextParam(this Room r, Param p, string value)
        {
            if (r.get_Parameter(p.Guid) != null)
                r.get_Parameter(p.Guid).Set(value);
        }
        public static void SetIntParam(this Room r, Param p, string value)
        {
            int i = 0;
            if(int.TryParse(value, out i))
            {
                if (r.get_Parameter(p.Guid) != null)
                    r.get_Parameter(p.Guid).Set(i);
            }
        }
        #endregion
        #region element
        private static void SetElementInstanceParam(this Element e, Param p, string value)
        {
            if (e.get_Parameter(p.Guid) != null)
            {
                Parameter par = e.get_Parameter(p.Guid);
                switch(p.Type)
                {
                    default:
                    case ParamType.Text:
                        par.Set(value);
                        break;
                    case ParamType.Double:
                        par.Set(double.Parse(value));
                        break;
                    case ParamType.Int:
                        par.Set(int.Parse(value));
                        break;
                }
            }
        }
        private static void SetElementTypeParam(this Element e, Param p, string value)
        {
            FamilySymbol s;
            s = e as FamilySymbol;
            if (s == null)
            {
                FamilyInstance i = e as FamilyInstance;
                s = i.Symbol;
            }
            if (s.get_Parameter(p.Guid) != null)
            {
                Parameter par = s.get_Parameter(p.Guid);
                switch (p.Type)
                {
                    default:
                    case ParamType.Text:
                        par.Set(value);
                        break;
                    case ParamType.Double:
                        par.Set(double.Parse(value));
                        break;
                    case ParamType.Int:
                        par.Set(int.Parse(value));
                        break;
                }
            }
        }
        private static ElementId GetMaterial(this Document doc, string Name)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var material = collector.WherePasses(new ElementClassFilter(typeof(Material))).ToElements().ToList().Where(x => x.Name == Name).First();
            return material.Id;
        }
        #endregion
    }
}