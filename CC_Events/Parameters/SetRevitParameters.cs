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
    internal static class SetRevitParams
    {
        public static void SetIDParam(this IDParam p, Document doc)
        {
            string id = Guid.NewGuid().ToString("N");
            if (doc.IsFamilyDocument)
                SetFamilyParam(p, doc, id);
            else
                SetProjectParam(p, doc, id);
            p.Value = id;
        }
        public static void SetFamilyParam(this Param p, Document doc, string value)
        {
            if (doc.IsFamilyDocument)
            {
                if (doc.FamilyManager.get_Parameter(p.ID) != null)
                {
                    FamilyParameter param = doc.FamilyManager.get_Parameter(p.ID);
                    foreach (FamilyType t in doc.FamilyManager.Types)
                    {
                        doc.FamilyManager.CurrentType = t;
                        doc.FamilyManager.Set(param, value);
                    }
                }
            }
        }
        public static void SetProjectParam(this Param p, Document doc, string value)
        {
            if (!doc.IsFamilyDocument)
                if (doc.ProjectInformation.get_Parameter(p.ID) != null)
                    doc.ProjectInformation.get_Parameter(p.ID).Set(value);
        }
        public static void SetRoomParam(this Param p, Room r, string value)
        {
            if (r.get_Parameter(p.ID) != null)
                r.get_Parameter(p.ID).Set(value);
        }
        public static void SetAreaParam(this Param p, Area a, string value)
        {
            if (a.get_Parameter(p.ID) != null)
                a.get_Parameter(p.ID).Set(value);
        }
        public static void SetWallParam(this Param p, Wall w, string value)
        {
            if (w.get_Parameter(p.ID) != null)
                w.get_Parameter(p.ID).Set(value);
        }
    }
}