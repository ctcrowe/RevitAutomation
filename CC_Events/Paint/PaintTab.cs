using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CC_Library;
using CC_Library.Parameters;
using CC_Plugin.Parameters;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using System.Reflection;

namespace CC_Plugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class PaintObjectByFinishMat : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            CCPaintPanel.PaintByMaterial(uiDoc, Params.Finish);
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class AddObjectStyles : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            using (Transaction t = new Transaction(doc, "Add Cats"))
            {
                t.Start();
                for(int i = 0; i < Enum.GetNames(typeof(ObjectCategory)).Length; i++)
                {
                    doc.AddCategories(i);
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class OrganizeFamilies : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            FamilyReorganize.Run();
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class LearnLineWeights : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            LineWeights.Learn(doc);
            return Result.Succeeded;
        }
    }
    internal class CCPaintPanel
    {
        //https://www.revitapidocs.com/2015/f59f8872-e8d7-5d00-0e8c-44a36a843861.htm
        //create a paint all surfaces tool.
        private static string dllpath = Assembly.GetExecutingAssembly().Location;
        public static void PaintPanel(UIControlledApplication uiApp)
        {
            RibbonPanel Panel = uiApp.CreateRibbonPanel(CCRibbon.tabName, "Paint");

            PushButtonData b1Data = new PushButtonData(
                "Paint All Surfaces",
                "Paint All\r\nSurfaces",
                @dllpath,
                "CC_Plugin.PaintObjectByFinishMat");
            b1Data.ToolTip = "Paint all Surfaces of an Object a the Finish Material Parameter";
            PushButton PB1 = Panel.AddItem(b1Data) as PushButton;
            
            PushButtonData b2Data = new PushButtonData(
                "Add All Categories",
                "Add All\r\nCategories",
                @dllpath,
                "CC_Plugin.AddObjectStyles");
            b2Data.ToolTip = "Add all predefined subcategories to the document.";
            PushButton PB2 = Panel.AddItem(b2Data) as PushButton;

            PushButtonData b3Data = new PushButtonData(
                "Organize Families",
                "Organize\r\nFamilies",
                @dllpath,
                "CC_Plugin.OrganizeFamilies");
            b3Data.ToolTip = "Organize all families in the mydocuments folder.";
            PushButton PB3 = Panel.AddItem(b3Data) as PushButton;

            PushButtonData b4Data = new PushButtonData(
                "Learn Line Weights",
                "Learn\r\nLine Weights",
                @dllpath,
                "CC_Plugin.LearnLineWeights");
            b4Data.ToolTip = "Learn from the line weights of the current view";
            PushButton PB4 = Panel.AddItem(b4Data) as PushButton;
        }
        public static void PaintByMaterial(UIDocument uidoc, Param par)
        {
            Document doc = uidoc.Document;
            if (doc.IsFamilyDocument)
            {
                Selection sel = uidoc.Selection;
                ISelectionFilter selectionFilter = new GFSelectionFilter();

                Reference ChangedObject = sel.PickObject(ObjectType.Element, selectionFilter);
                GenericForm gf = doc.GetElement(ChangedObject.ElementId) as GenericForm;

                using (Transaction t = new Transaction(doc, "Paint Faces"))
                {
                    t.Start();
                    FamilyManager fmgr = doc.FamilyManager;

                    FamilyParameter p;
                    if (doc.FamilyManager.get_Parameter(par.Guid) == null)
                    {
                        ExternalDefinition def = par.CreateDefinition(doc) as ExternalDefinition;
                        p = doc.FamilyManager.AddParameter(def, BuiltInParameterGroup.PG_IFC, par.Instance);
                    }
                    else
                        p = doc.FamilyManager.get_Parameter(par.Guid);

                    Options geoOptions = new Options();
                    geoOptions.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement geoEle = gf.get_Geometry(geoOptions);

                    IEnumerator<GeometryObject> geoObjIt = geoEle.GetEnumerator();
                    while (geoObjIt.MoveNext())
                    {
                        Solid solid = geoObjIt.Current as Solid;
                        if (solid != null)
                        {
                            foreach (Face f in solid.Faces)
                            {
                                doc.Paint(gf.Id, f, p);
                            }
                        }
                    }
                    t.Commit();
                }
            }
        }
        public class GFSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                GenericForm gf = element as GenericForm;
                if (gf != null)
                    return true;
                else
                    return false;
            }
            public bool AllowReference(Reference refer, XYZ point) { return true; }
        }
    }
}
