using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

using CC_Library;
using CC_Library.Predictions;

namespace CC_Plugin
{
    internal static class CollectObjStyles
    {
        private static string lwfilename = "ImportedObjectStlyes_Lineweights.csv".GetMyDocs();
        private static string ltfilename = "ImportedObjectStlyes_LineTypes.csv".GetMyDocs();

        private static void UpdateObjectStyle(this Category c, ElementId[] eids)
        {
            string name = c.Name;
            c.LineColor = new Color(0, 0, 0);
            c.SetLineWeight(1, GraphicsStyleType.Projection);
            c.Material = null;
            c.SetLinePatternId(eids[0], GraphicsStyleType.Projection);
        }
        public static void CollectStyles(this Document doc)
        {
            List<string> lwstrings = new List<string>();
            List<string> ltstrings = new List<string>();
            Categories cats = doc.Settings.Categories;
            foreach (Category c in cats)
            {
                if (c.IsImportedCategory())
                {
                    if (!c.SubCategories.IsEmpty)
                    {
                        foreach (Category cs in c.SubCategories)
                        {
                            var pattern = doc.GetElement(cs.GetLinePatternId(GraphicsStyleType.Projection)) as LinePatternElement;
                            if (pattern != null)
                            {
                                if (Names.Contains(pattern.Name))
                                {

                                    string lt = cs.Parent.Name.SimplifyTitle() + "_" + cs.Name.SimplifyTitle() + "," + Names.ToList().IndexOf(pattern.Name);
                                    ltstrings.Add(lt);
                                }
                            }
                            string lw = cs.Parent.Name.SimplifyTitle() + "_" + cs.Name.SimplifyTitle() + "," + cs.GetLineWeight(GraphicsStyleType.Projection);
                            lwstrings.Add(lw);
                        }
                    }
                }
            }
            File.WriteAllLines(lwfilename, lwstrings);
            File.WriteAllLines(ltfilename, ltstrings);
        }
        public static void UpdateStyles(this Document doc)
        {
            Categories cats = doc.Settings.Categories;
            var Patterns = doc.GetLineStyles();
            using (TransactionGroup tg = new TransactionGroup(doc, "Line Style Changes"))
            {
                ElementId[] eids = new ElementId[5];
                using (Transaction t = new Transaction(doc, "Get EIDs"))
                {
                    t.Start();
                    eids = GetLineStyles(doc);
                    t.Commit();
                }
                tg.Start();
                foreach (Category c in cats)
                {
                    using (Transaction t = new Transaction(doc, "Set Line Styles"))
                    {
                        t.Start();
                        if (c.IsImportedCategory())
                        {
                            c.UpdateObjectStyle(eids);

                            if (!c.SubCategories.IsEmpty)
                            {
                                foreach (Category cs in c.SubCategories)
                                    cs.UpdateObjectStyle(eids);
                            }
                        }
                        t.Commit();
                    }
                }
                tg.Commit();
            }
        }
        private static bool IsImportedCategory(this Category category)
        {
            if (Enum.IsDefined(typeof(BuiltInCategory), category.Id.IntegerValue))
                return false;

            if (category.Id.IntegerValue < -1)
                return false;

            return category.AllowsBoundParameters == false &&
                category.CanAddSubcategory == false &&
                category.HasMaterialQuantities == false &&
                category.IsCuttable == false;
        }
        private static ElementId[] GetLineStyles(this Document doc)
        {
            ElementId[] eids = new ElementId[5];
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(LinePatternElement))
                .ToElementIds().ToList();
            bool[] check = new bool[5] { false, false, false, false, false };
            foreach(var id in collector)
            {
                LinePatternElement lpe = doc.GetElement(id) as LinePatternElement;
                string name = lpe.Name;
                if (Names.Contains(name))
                {
                    check[Names.ToList().IndexOf(name)] = true;
                    eids[Names.ToList().IndexOf(name)] = lpe.Id;
                }
            }
            if(!check[0])
            {
                eids[0] = LinePatternElement.GetSolidPatternId();
            }
            if(!check[1])
            {
                TaskDialog.Show("Test", "Dash Trying to be Created");
                LinePattern lp = new LinePattern("Dash");
                lp.SetSegments(CreateDash());
                LinePatternElement lpe = LinePatternElement.Create(doc, lp);
                eids[1] = lpe.Id;
            }
            if(!check[2])
            {
                LinePattern lp = new LinePattern("Dot");
                lp.SetSegments(CreateDot());
                LinePatternElement lpe = LinePatternElement.Create(doc, lp);
                eids[2] = lpe.Id;
            }
            if(!check[3])
            {
                LinePattern lp = new LinePattern("Center");
                lp.SetSegments(CreateCenter());
                LinePatternElement lpe = LinePatternElement.Create(doc, lp);
                eids[3] = lpe.Id;
            }
            if(!check[4])
            {
                LinePattern lp = new LinePattern("Hidden");
                lp.SetSegments(CreateHidden());
                LinePatternElement lpe = LinePatternElement.Create(doc, lp);
                eids[4] = lpe.Id;
            }
            return eids;
        }
        private static IList<LinePatternSegment> CreateDash()
        {
            List<LinePatternSegment> Segments = new List<LinePatternSegment>();
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.05));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.05));
            return Segments;
        }
        private static IList<LinePatternSegment> CreateDot()
        {
            List<LinePatternSegment> Segments = new List<LinePatternSegment>();
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Dot, 0.01));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.05));
            return Segments;
        }
        private static IList<LinePatternSegment> CreateCenter()
        {
            List<LinePatternSegment> Segments = new List<LinePatternSegment>();
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.05));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.05));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Dot, 0.01));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.05));
            return Segments;
        }
        private static IList<LinePatternSegment> CreateHidden()
        {
            List<LinePatternSegment> Segments = new List<LinePatternSegment>();
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.02));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));
            return Segments;
        }
        public static string[] Names = new string[5] { "Solid", "Dash", "Dot", "Center", "Hidden" };
    }

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class GetObjStyles : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document currentDoc = uiDoc.Document;
            currentDoc.CollectStyles();
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class SetObjStyles : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document currentDoc = uiDoc.Document;
            currentDoc.UpdateStyles();
            return Result.Succeeded;
        }
    }
}
