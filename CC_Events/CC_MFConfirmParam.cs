﻿using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace CC_Plugin
{
    internal class MFConfirmParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "MasterFormat_Confirmed",
                    /* Type */              ParamType.Bool,
                    /* ID */                new Guid("98d1341d-cf2a-4701-8f3b-772496234477"),
                    /* Param Group */       "BIMData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_GenericModel},
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "CONFIRMS IF THE MASTERFORMAT DIVISION WAS SET AUTOMATICALLY OR THROUGH THE API.",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static void Add(Document doc) { RevitParamEdits.Add_FamilyParam(doc, P); }
        public static bool Get(Document doc) { return RevitParamEdits.Get_BooleanFamilyParam(doc, P); }
        public static string Set(Document doc, int i)
        {
            Add(doc);
            return P.Set(doc, i.ToString());
        }
    }
    internal class WidthParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "oW",
                    /* Type */              ParamType.Length,
                    /* ID */                new Guid("7c53c6ed-278f-4036-b5c7-eac8437ab28a"),
                    /* Param Group */       "Dimensions",
                    /* Categories */        new BuiltInCategory[8] {
                                                BuiltInCategory.OST_GenericModel,
                                                BuiltInCategory.OST_SpecialityEquipment,
                                                BuiltInCategory.OST_Furniture,
                                                BuiltInCategory.OST_FurnitureSystems,
                                                BuiltInCategory.OST_Walls,
                                                BuiltInCategory.OST_Windows,
                                                BuiltInCategory.OST_Doors,
                                                BuiltInCategory.OST_Casework },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_CONSTRUCTION,
                    /* Description */       "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING LEFT AND RIGHT EXTREMES",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   true,
                    /* Fixed */             true);
            }
        }
        public static void Add(Document doc) { RevitParamEdits.Add_FamilyParam(doc, P); }
        public static string Set(Document doc, int i)
        {
            Add(doc);
            return P.Set(doc, i.ToString());
        }
    }
    internal class DepthParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "oD",
                    /* Type */              ParamType.Length,
                    /* ID */                new Guid("93acc448-a229-4d27-956f-c39ffd40c3c3"),
                    /* Param Group */       "Dimensions",
                    /* Categories */        new BuiltInCategory[8] {
                                                BuiltInCategory.OST_GenericModel,
                                                BuiltInCategory.OST_SpecialityEquipment,
                                                BuiltInCategory.OST_Furniture,
                                                BuiltInCategory.OST_FurnitureSystems,
                                                BuiltInCategory.OST_Walls,
                                                BuiltInCategory.OST_Windows,
                                                BuiltInCategory.OST_Doors,
                                                BuiltInCategory.OST_Casework },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_CONSTRUCTION,
                    /* Description */       "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING FRONT AND BACK EXTREMES",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   true,
                    /* Fixed */             true);
            }
        }
        public static void Add(Document doc) { RevitParamEdits.Add_FamilyParam(doc, P); }
        public static string Set(Document doc, int i)
        {
            Add(doc);
            return P.Set(doc, i.ToString());
        }
    }
    internal class HeightParam
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        private static Param P
        {
            get
            {
                return new Param(
                    /* Name */              "oH",
                    /* Type */              ParamType.Length,
                    /* ID */                new Guid("f306ae6d-f153-47e1-b7e1-ac451eada6f2"),
                    /* Param Group */       "Dimensions",
                    /* Categories */        new BuiltInCategory[8] {
                                                BuiltInCategory.OST_GenericModel,
                                                BuiltInCategory.OST_SpecialityEquipment,
                                                BuiltInCategory.OST_Furniture,
                                                BuiltInCategory.OST_FurnitureSystems,
                                                BuiltInCategory.OST_Walls,
                                                BuiltInCategory.OST_Windows,
                                                BuiltInCategory.OST_Doors,
                                                BuiltInCategory.OST_Casework },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_CONSTRUCTION,
                    /* Description */       "USED TO DEFINE THE OVERALL OUTSIDE MAXIMUM DIMENSIONS OF AN OBJECT, REFERENCING TOP AND BOTTOM EXTREMES",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   true,
                    /* Fixed */             true);
            }
        }
        public static void Add(Document doc) { RevitParamEdits.Add_FamilyParam(doc, P); }
        public static string Set(Document doc, int i)
        {
            Add(doc);
            return P.Set(doc, i.ToString());
        }
    }
}