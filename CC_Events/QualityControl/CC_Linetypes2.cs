using System.Collections.Generic;
using System.Linq;
using System;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace CC_Plugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class SelectLines : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document currentDoc = uiDoc.Document;
            using (Transaction trans = new Transaction(currentDoc))
            {
                trans.Start("LineStyles");
                Linetypes.SelectAllLines(uiDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ChangeLineStyle : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document currentDoc = uiDoc.Document;
            Linetypes.EditLinetypes(uiDoc);
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ListLineStyles : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document currentDoc = uiDoc.Document;
            using (Transaction trans = new Transaction(currentDoc))
            {
                trans.Start("LineStyles");
                Linetypes.ListLineStyles(uiDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    public class Linetypes
    {
        public static void ListCategory(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            Selection sel = uiDoc.Selection;

            Reference Object = sel.PickObject(ObjectType.Element);
            Element ele = currentDoc.GetElement(Object.ElementId);
            BuiltInCategory mycatenum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), ele.Category.Id.IntegerValue.ToString());
            TaskDialog.Show("Category", ele.Category.Name + "\r\n" + mycatenum.ToString());
        }
        public static void EditLinetypes(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            using (TransactionGroup tgroup = new TransactionGroup(currentDoc, "EditLineTypes"))
            {
                tgroup.Start();
                Selection sel = uiDoc.Selection;
                ISelectionFilter selectionFilter = new LineSelectionFilter();

                Reference ChangedObject = sel.PickObject(ObjectType.Element, selectionFilter);
                CurveElement ChangedElement = currentDoc.GetElement(ChangedObject.ElementId) as CurveElement;

                Reference NewObject = sel.PickObject(ObjectType.Element, selectionFilter);
                CurveElement NewElement = currentDoc.GetElement(NewObject.ElementId) as CurveElement;
                GraphicsStyle style = NewElement.LineStyle as GraphicsStyle;

                Category linesCat = currentDoc.Settings.Categories.get_Item("Lines");
                Category cat = linesCat.SubCategories.Cast<Category>().Where(c => c.Name == ChangedElement.LineStyle.Name).First();
                ElementId OldLinestyleID = cat.Id;

                FilteredElementCollector lineCollector = new FilteredElementCollector(currentDoc);
                List<ElementId> Lines = lineCollector.OfCategory(BuiltInCategory.OST_Lines).ToElementIds().ToList();

                foreach (ElementId EID in Lines)
                {
                    CurveElement CE = currentDoc.GetElement(EID) as CurveElement;
                    if (CE.LineStyle.Name == cat.Name)
                    {
                        if (CE.GroupId == ElementId.InvalidElementId)
                        {
                            using (Transaction ungroupedtrans = new Transaction(currentDoc, "ungrouped transaction"))
                            {
                                ungroupedtrans.Start();
                                CE.LineStyle = style;
                                ungroupedtrans.Commit();
                            }
                        }
                        else
                        {
                            Group group = currentDoc.GetElement(CE.GroupId) as Group;
                            string gn = group.GroupType.Name;
                            if (group.GroupId == ElementId.InvalidElementId)
                            {
                                Dictionary<ElementId, GroupType> groupadjustments = new Dictionary<ElementId, GroupType>();
                                ElementId GroupTypeId = group.GroupType.Id;
                                if (groupadjustments.Keys.Contains(GroupTypeId))
                                {
                                    using (Transaction GroupTrans = new Transaction(currentDoc, "grouped transaction"))
                                    {
                                        GroupTrans.Start();
                                        FailureHandlingOptions options = GroupTrans.GetFailureHandlingOptions();
                                        options.SetFailuresPreprocessor(new DiscardWarnings());
                                        GroupTrans.SetFailureHandlingOptions(options);

                                        group.GroupType = groupadjustments[GroupTypeId];
                                        GroupTrans.Commit();
                                    }
                                }
                                else
                                {
                                    using (Transaction GroupTrans = new Transaction(currentDoc, "grouped transaction"))
                                    {
                                        GroupTrans.Start();
                                        FailureHandlingOptions options = GroupTrans.GetFailureHandlingOptions();
                                        options.SetFailuresPreprocessor(new DiscardWarnings());
                                        GroupTrans.SetFailureHandlingOptions(options);

                                        ICollection<ElementId> set = group.UngroupMembers();
                                        GroupTrans.Commit();
                                        using (Transaction Group2Trans = new Transaction(currentDoc, "grouped transaction 2"))
                                        {
                                            Group2Trans.Start();
                                            FailureHandlingOptions SubGroupOptions = Group2Trans.GetFailureHandlingOptions();
                                            SubGroupOptions.SetFailuresPreprocessor(new DiscardWarnings());
                                            Group2Trans.SetFailureHandlingOptions(SubGroupOptions);
                                            CE.LineStyle = style;
                                            Group newgroup = currentDoc.Create.NewGroup(set);
                                            newgroup.GroupType.Name = gn + "_New";
                                            Group2Trans.Commit();
                                            groupadjustments.Add(GroupTypeId, newgroup.GroupType);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                using (Transaction deletetrans = new Transaction(currentDoc, "Delete Transaction"))
                {
                    deletetrans.Start();
                    currentDoc.Delete(OldLinestyleID);
                    deletetrans.Commit();
                }
                tgroup.Assimilate();
            }
        }
        public static void ListLineStyles(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            XYZ BasePt = sel.PickPoint();

            FilteredElementCollector LineCollector = new FilteredElementCollector(currentDoc);
            List<ElementId> Lines = LineCollector.OfCategory(BuiltInCategory.OST_Lines).ToElementIds().ToList();
            FilteredElementCollector txtCollector = new FilteredElementCollector(currentDoc);
            ElementId txtnoteTypeId = txtCollector.OfCategory(BuiltInCategory.OST_TextNotes).FirstElement().GetTypeId();

            int adjustment = 10;
            Dictionary<string, CurveElement> LineStyles = new Dictionary<string, CurveElement>();
            for (int i = 0; i < Lines.Count(); i++)
            {
                CurveElement l = currentDoc.GetElement(Lines[i]) as CurveElement;
                if (!LineStyles.Keys.Contains(l.LineStyle.Name))
                {
                    LineStyles.Add(l.LineStyle.Name, l);
                }
            }
            List<string> StyleName = LineStyles.Keys.ToList();
            StyleName.Sort();
            for(int i = 0; i < StyleName.Count(); i++)
            {
                XYZ startpoint = new XYZ(BasePt.X, (BasePt.Y - (adjustment * i)), 0);
                XYZ endpoint = new XYZ((BasePt.X + (adjustment * 3)), (BasePt.Y - (adjustment * i)), 0);
                XYZ TextPoint = new XYZ(BasePt.X + (adjustment * 4), (BasePt.Y - (adjustment * i)), 0);

                Curve baseCurve = Line.CreateBound(startpoint, endpoint) as Curve;
                DetailCurve addedLine = currentDoc.Create.NewDetailCurve(uiDoc.ActiveView, baseCurve);

                TextNote txNote = TextNote.Create(currentDoc, uiDoc.ActiveView.Id, TextPoint, StyleName[i], txtnoteTypeId);
                addedLine.LineStyle = LineStyles[StyleName[i]].LineStyle;
            }
        }
        public static void SelectAllLines(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            Selection Sel = uiDoc.Selection;
            ElementId v = uiDoc.ActiveView.Id;
            ISelectionFilter selectionFilter = new LineSelectionFilter();

            Reference ChangedObject = Sel.PickObject(ObjectType.Element, selectionFilter);
            CurveElement ChangedElement = currentDoc.GetElement(ChangedObject.ElementId) as CurveElement;

            FilteredElementCollector lineCollector = new FilteredElementCollector(currentDoc, v);
            ICollection<ElementId> lines = lineCollector.OfCategory(BuiltInCategory.OST_Lines).ToElementIds().ToList();

            List<ElementId> linelist = new List<ElementId>();
            foreach(ElementId eid in lines)
            {
                CurveElement ce = currentDoc.GetElement(eid) as CurveElement;
                if(ce.LineStyle.Name == ChangedElement.LineStyle.Name)
                {
                    linelist.Add(eid);
                }
            }
            Sel.SetElementIds(linelist);
        }
    }
    public class LineSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
            {

                if(element.Category.Name == "Lines")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        public bool AllowReference(Reference refer, XYZ point)
            {
                return true;
            }
    }
    public class DiscardWarnings : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> FailureList = new List<FailureMessageAccessor>();
            FailureList = failuresAccessor.GetFailureMessages();
            foreach(FailureMessageAccessor failure in FailureList)
            {
                FailureDefinitionId failID = failure.GetFailureDefinitionId();
                if(failID == BuiltInFailures.GroupFailures.AtomViolationWhenOnePlaceInstance)
                {
                    failuresAccessor.DeleteWarning(failure);
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}
