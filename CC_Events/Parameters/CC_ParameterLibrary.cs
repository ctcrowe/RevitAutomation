using System;
using Autodesk.Revit.DB;

namespace CC_Plugin
{
    public class ParameterLibrary
    {
        public static Param FamilyCheck
        {
            get
            {
                return new Param(
                    /* Name */              "FamCheck",
                    /* Type */              ParamType.Bool,
                    /* ID */                new Guid("4c901283-e4fb-470c-b764-76661b62c162"),
                    /* Param Group */       "FamilyData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_Casework },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_TEXT,
                    /* Description */       "A check to see if families have already been loaded into the selected family",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param FDataGeom
        {
            get
            {
                return new Param(
                    /* Name */              "FDataGeom",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("9d522752-b528-4ade-81d3-80e68d67c15e"),
                    /* Param Group */       "FamilyData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_GenericModel },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF GENERIC FORMS IN THE FAMILY",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param FDataCat
        {
            get
            {
                return new Param(
                    /* Name */              "FDataCategory",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("eba8e985-c084-41cb-ab6f-6274eb705138"),
                    /* Param Group */       "FamilyData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_GenericModel },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A REFERENCE TO THE CATEGORY OF THE FAMILY",
                    /* Visible */           true,
                    /* Instance */          false,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param OverallDepth
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
        public static Param OverallHeight
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
        public static Param OverallWidth
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
        public static Param PDataFamilies
        {
            get
            {
                return new Param(
                    /* Name */              "PDataFamilies",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("bdffd1f0-c8d1-4f53-b1bc-9a4d8d303ff5"),
                    /* Param Group */       "ProjectData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_ProjectInformation },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF FAMILIES IN THE PROJECT",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param PDataFamilyInstances
        {
            get
            {
                return new Param(
                    /* Name */              "PDataFamilyInstances",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("e17502bc-853b-449e-932c-363abd958912"),
                    /* Param Group */       "ProjectData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_ProjectInformation },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF FAMILY INSTANCES IN THE PROJECT",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param PDataMaterials
        {
            get
            {
                return new Param(
                    /* Name */              "PDataMaterials",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("6c183e2b-46e1-4608-af59-ee133d2dd197"),
                    /* Param Group */       "ProjectData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_ProjectInformation },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF MATERIALS IN THE PROJECT",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param PDataRefPlanes
        {
            get
            {
                return new Param(
                    /* Name */              "PDataRefPlanes",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("2dd87d8d-5d67-4971-ad2c-159d0cb5631d"),
                    /* Param Group */       "ProjectData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_ProjectInformation },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF REFERENCE PLANES IN THE PROJECT",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param PDataSheets
        {
            get
            {
                return new Param(
                    /* Name */              "PDataSheets",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("28f0bc4e-d228-4c98-bbd7-fb8808f98d1d"),
                    /* Param Group */       "ProjectData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_ProjectInformation },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF SHEETS IN THE PROJECT",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param PDataViews
        {
            get
            {
                return new Param(
                    /* Name */              "PDataViews",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("025280e8-5e53-4bae-bcf0-30f9245ccba0"),
                    /* Param Group */       "ProjectData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_ProjectInformation },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF VIEWS IN THE PROJECT",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param PDataVoS
        {
            get
            {
                return new Param(
                    /* Name */              "PDataVoS",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("28c7842a-1306-4a07-809c-d75c35ea8e26"),
                    /* Param Group */       "ProjectData",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_ProjectInformation },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "A COUNT OF THE NUMBER OF VIEWS PLACED ON SHEETS IN THE PROJECT",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   false,
                    /* Fixed */             true);
            }
        }
        public static Param Privacy
        {
            get
            {
                return new Param(
                    /* Name */              "Privacy",
                    /* Type */              ParamType.Int,
                    /* ID */                new Guid("c41bb428-8abf-45a9-82d1-6bd991ece297"),
                    /* Param Group */       "Rooms",
                    /* Categories */        new BuiltInCategory[1] {
                                                BuiltInCategory.OST_Rooms },
                    /* BuiltinParamGroup*/  BuiltInParameterGroup.PG_DATA,
                    /* Description */       "USED FOR COLOR FILL DATA FOR AND FILTERING INFORMATION VIA WHERE IT MIGHT BE. SCALE OF -5 TO 5, NEGATIVE NUMBERS REPRESENT EXTERIOR ELEMENTS",
                    /* Visible */           true,
                    /* Instance */          true,
                    /* User Modifiable */   true,
                    /* Fixed */             true);
            }
        }
    }
}