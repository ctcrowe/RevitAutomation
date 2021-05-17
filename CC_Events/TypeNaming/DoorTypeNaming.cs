using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using CC_Library.Parameters;
using CC_Plugin.Parameters;

namespace CC_Plugin.TypeNaming
{
    internal static class DoorTypes
    {
        public static string SetDoorTypeName(this Element ele)
        {
            string TypeName = "";
            FamilySymbol symb = ele as FamilySymbol;
            Category cat = symb.Family.FamilyCategory;
            if((int)BuiltInCategory.OST_Doors == cat.Id.IntegerValue)
            {
                string Width = ele.GetElementParam(DoorParams.PanelWidth);
                string Height = ele.GetElementParam(DoorParams.PanelHeight);
                string Frame = ele.GetElementParam(DoorParams.FrameMaterial);
                string Door = ele.GetElementParam(DoorParams.PanelMaterial);

                if (Door != null)
                    TypeName += "D " + Door + " ";
                if (Frame != null)
                    TypeName += "F " + Frame + " ";
                if (Height != null)
                    TypeName += Height + " H ";
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
    }

}
