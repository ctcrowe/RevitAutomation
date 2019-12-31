using System.Collections.Generic;
using System.Linq;
using System;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

using CC_CoreData;

namespace CC_Events
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
            string id = "";
            if(ele.get_Parameter(ParameterLibrary.FamilyID.ID) != null)
            {
                id = ele.get_Parameter(ParameterLibrary.FamilyID.ID).AsString();
            }
            TaskDialog.Show("Category", ele.Category.Name + "\r\n" + mycatenum.ToString() + "/r/n" + id);
        }
        public static void EditLinetypes(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            GroupUpdater grps = new GroupUpdater();
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
                            grps.UpdateGroup(group, CE, style);
                        }
                    }
                }
                grps.Regroup(currentDoc);
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

                Curve baseCurve = Autodesk.Revit.DB.Line.CreateBound(startpoint, endpoint) as Curve;
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
    public class GroupContainer
    {
        public int groupnum { get; set; }
        public string groupname { get; set; }
        public ICollection<ElementId> groupset { get; set; }
        public GroupContainer(int groupno, string gn, ICollection<ElementId> set)
        {
            groupnum = groupno;
            groupname = gn;
            groupset = set;
        }
    }
    public class GroupUpdater
    {
        public Dictionary<ElementId, GroupType> grouptypes { get; set; }
        public List<GroupContainer> groupsets { get; set; }
        public int GroupNumIterator { get; set; }
        public GroupUpdater()
        {
            grouptypes = new Dictionary<ElementId, GroupType>();
            groupsets = new List<GroupContainer>();
            GroupNumIterator = 0;
        }
        public void UpdateGroup(Group grp, CurveElement CE, GraphicsStyle Style)
        {
            Document doc = grp.Document;
            string gn = grp.GroupType.Name;
            ElementId GroupTypeId = grp.GroupType.Id;
            if (grouptypes.Keys.Contains(GroupTypeId))
            {
                using (Transaction t1 = new Transaction(doc, "Group Transaction"))
                {
                    t1.Start();
                    FailureHandlingOptions options = t1.GetFailureHandlingOptions();
                    options.SetFailuresPreprocessor(new DiscardWarnings());
                    t1.SetFailureHandlingOptions(options);

                    grp.GroupType = grouptypes[GroupTypeId];
                    t1.Commit();
                }
            }
            else
            {
                if (grp.GroupId == ElementId.InvalidElementId)
                {
                    using (Transaction t1 = new Transaction(doc, "Group Transaction"))
                    {
                        t1.Start();
                        FailureHandlingOptions options = t1.GetFailureHandlingOptions();
                        options.SetFailuresPreprocessor(new DiscardWarnings());
                        t1.SetFailureHandlingOptions(options);

                        ICollection<ElementId> set = grp.UngroupMembers();
                        t1.Commit();
                        using (Transaction t2 = new Transaction(doc, "Group Transaction 2"))
                        {
                            t2.Start();
                            FailureHandlingOptions SubGroupOptions = t2.GetFailureHandlingOptions();
                            SubGroupOptions.SetFailuresPreprocessor(new DiscardWarnings());
                            t2.SetFailureHandlingOptions(SubGroupOptions);
                            CE.LineStyle = Style;
                            Group newgroup = doc.Create.NewGroup(set);
                            newgroup.GroupType.Name = gn + "_New";
                            t2.Commit();
                            grouptypes.Add(GroupTypeId, newgroup.GroupType);
                        }
                    }
                }
                else
                {
                    Group parentGroup = doc.GetElement(grp.GroupId) as Group;
                    UpdateGroup(parentGroup, CE, Style);
                    string GroupName = grp.Name;
                    using (Transaction t1 = new Transaction(doc, "Group Transaction 1"))
                    {
                        t1.Start();
                        FailureHandlingOptions GroupOptions = t1.GetFailureHandlingOptions();
                        GroupOptions.SetFailuresPreprocessor(new DiscardWarnings());
                        t1.SetFailureHandlingOptions(GroupOptions);

                        ICollection<ElementId> set = grp.UngroupMembers();
                        t1.Commit();

                        groupsets.Add(new GroupContainer(GroupNumIterator++, GroupName, set));
                    }
                }
            }
        }
        public void Regroup(Document doc)
        {
            for (int i = GroupNumIterator; i > 1; i--)
            {
                using (Transaction Trans = new Transaction(doc, "Regroup Transaction"))
                {
                    Trans.Start();
                    FailureHandlingOptions FHO = Trans.GetFailureHandlingOptions();
                    FHO.SetFailuresPreprocessor(new DiscardWarnings());
                    Trans.SetFailureHandlingOptions(FHO);

                    if (groupsets.Any(x => x.groupnum == i))
                    {
                        GroupContainer container = groupsets.Where(x => x.groupnum == i).First();
                        Group newGroup = doc.Create.NewGroup(container.groupset);
                        //newGroup.Name = container.groupname + "_new";

                        if(groupsets.Any(x => x.groupnum == i - 1))
                            groupsets.Where(x => x.groupnum == i - 1).First().groupset.Add(newGroup.Id);
                    }
                }
            }
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
