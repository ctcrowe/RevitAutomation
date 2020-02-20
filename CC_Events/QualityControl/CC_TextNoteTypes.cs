using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace CC_Plugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class SelectNotes : IExternalCommand
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
                TextTypes.SelectAllNotes(uiDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ChangeTextStyle : IExternalCommand
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
                trans.Start("TextStyles");
                TextTypes.EditTextStyles(uiDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ListTextStyles : IExternalCommand
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
                trans.Start("TextStyles");
                TextTypes.ListTextStyles(uiDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    public class TextTypes
    {
        public static void EditTextStyles(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            ISelectionFilter selectionFilter = new TextSelectionFilter();

            Reference ChangedObject = sel.PickObject(ObjectType.Element, selectionFilter);
            TextNote ChangedElement = currentDoc.GetElement(ChangedObject.ElementId) as TextNote;

            Reference NewObject = sel.PickObject(ObjectType.Element, selectionFilter);
            TextNote NewElement = currentDoc.GetElement(NewObject.ElementId) as TextNote;
            TextNoteType type = NewElement.TextNoteType;

            TextNoteType oldType = ChangedElement.TextNoteType;

            FilteredElementCollector TextCollector = new FilteredElementCollector(currentDoc);
            List<ElementId> Notes = TextCollector.OfCategory(BuiltInCategory.OST_TextNotes).ToElementIds().ToList();

            foreach(ElementId EID in Notes)
            {
                TextNote TN = currentDoc.GetElement(EID) as TextNote;
                if (TN.TextNoteType.Name == oldType.Name)
                {
                    if (TN.GroupId == ElementId.InvalidElementId)
                    {
                        TN.TextNoteType = type;
                    }
                    else
                    {
                        Group group = currentDoc.GetElement(TN.GroupId) as Group;
                    }
                }
            }
            
            currentDoc.Delete(oldType.Id);
        }
        public static void ListTextStyles(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            XYZ BasePt = sel.PickPoint();

            FilteredElementCollector TextCollector = new FilteredElementCollector(currentDoc);
            List<ElementId> Notes = TextCollector.OfCategory(BuiltInCategory.OST_TextNotes).ToElementIds().ToList();

            int adjustment = 10;
            Dictionary<string, TextNote> TextNotes = new Dictionary<string, TextNote>();
            for (int i = 0; i < Notes.Count(); i++)
            {
                TextNote tn = currentDoc.GetElement(Notes[i]) as TextNote;
                if (!TextNotes.Keys.Contains(tn.TextNoteType.Name))
                {
                    TextNotes.Add(tn.TextNoteType.Name, tn);
                }
            }
            List<string> StyleName = TextNotes.Keys.ToList();
            StyleName.Sort();
            for(int i = 0; i < StyleName.Count(); i++)
            {
                XYZ TextPoint = new XYZ(BasePt.X, (BasePt.Y - (adjustment * i)), 0);
                TextNote txNote = TextNote.Create(currentDoc, uiDoc.ActiveView.Id, TextPoint, StyleName[i], TextNotes[StyleName[i]].GetTypeId());
            }
        }
        public static void SelectAllNotes(UIDocument uiDoc)
        {
            Document currentDoc = uiDoc.Document;
            Selection Sel = uiDoc.Selection;
            ElementId v = uiDoc.ActiveView.Id;
            ISelectionFilter selectionFilter = new TextSelectionFilter();

            Reference ChangedObject = Sel.PickObject(ObjectType.Element, selectionFilter);
            TextNote ChangedElement = currentDoc.GetElement(ChangedObject.ElementId) as TextNote;

            FilteredElementCollector TextCollector = new FilteredElementCollector(currentDoc, v);
            ICollection<ElementId> notes = TextCollector.OfCategory(BuiltInCategory.OST_TextNotes).ToElementIds().ToList();

            List<ElementId> notelist = new List<ElementId>();
            foreach(ElementId eid in notes)
            {
                TextNote tn = currentDoc.GetElement(eid) as TextNote;
                if(tn.TextNoteType.Name == ChangedElement.TextNoteType.Name)
                {
                    notelist.Add(eid);
                }
            }
            Sel.SetElementIds(notelist);
        }
    }
    public class TextSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
            {

                if(element.Category.Name == "Text Notes")
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
}
