using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using CC_Library.Parameters;
using CC_Plugin.Parameters;

namespace CC_Plugin.TypeNaming
{
    internal static class WindowTypes
    {
        public static string SetWindowTypeName(this Element ele)
        {
            string TypeName = "";
            FamilySymbol symb = ele as FamilySymbol;
            Category cat = symb.Family.FamilyCategory;
            if((int)BuiltInCategory.OST_Windows == cat.Id.IntegerValue)
            {
                string tm = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_MARK).AsString();
                string Height = ele.get_Parameter(BuiltInParameter.WINDOW_HEIGHT).AsValueString();
                string Width = ele.get_Parameter(BuiltInParameter.WINDOW_WIDTH).AsValueString();

                if (tm != null)
                    TypeName += "Type " + tm;
                if (Height != null)
                    TypeName += " - " + Height + " H ";
                if(Height != null && Width != null)
                {
                    TypeName += "x " + Width + " W";
                }
                else
                {
                    if (Width != null)
                        TypeName += Width + " W";
                }
                
                if(ele.Name != TypeName)
                    ele.Name = TypeName;
            }
            return TypeName;
        }
        public static string SetCaseworkTypeName(this Element ele)
        {
            string TypeName = "";
            FamilySymbol symb = ele as FamilySymbol;
            Category cat = symb.Family.FamilyCategory;
            if((int)BuiltInCategory.OST_Casework == cat.Id.IntegerValue)
            {
                
            }
            return TypeName;
        }
    }

}
