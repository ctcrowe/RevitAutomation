using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin.TypeNaming
{
    internal static class WallTypes
    {
        public static string SetWallTypeName(this Element ele)
        {
            string TypeName = "";
            string Type = null;
            string Thickness = null;
            string StudSize = null;

            WallType wt = ele as WallType;
            Category cat = wt.Category;
            if((int)BuiltInCategory.OST_Walls == cat.Id.IntegerValue)
            {
                try { Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_MARK).AsString(); } catch { TaskDialog.Show("Error", "Failed to retrieve Type Mark"); }
                try { Thickness = ele.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsValueString(); } catch { TaskDialog.Show("Error", "Failed to retrieve thickness"); }
                //try { StudSize = UpdateStudSize.SetWallThickness(ele); } catch { TaskDialog.Show("Eror", "Failed to retrieve Stud Size"); }

                if (Type != null)
                    TypeName += "Type " + Type;
                if (StudSize != null)
                    TypeName += " " + StudSize + " Studs";
                if (Thickness != null)
                    TypeName += " - OT " + Thickness;
                
                if(ele.Name != TypeName)
                    ele.Name = TypeName;
            }
            return TypeName;
        }
    }

}