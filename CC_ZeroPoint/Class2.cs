using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodest.Revit.UI;

namespace CC_ZeroPoint
{
    public class GetZeroPointBox
    {
        private const double Width = 7.375 / 12;
        private const double Height = 7.25 / 12;
        private const double Title = 1.25 / 12;
        private const double Notes = 2.125 / 12;
        
        public static void CreateBox(UIControlledApplication app)
        {
            var doc = app.ActiveUIDocument.Document;
            var view = doc.ActiveView;
            
            if(view.ViewType == ViewType.DraftingView)
            {
                var scale = view.Scale;
                var multiplier = GetComboData(app);
                var adjw = Width * scale * multiplier[1];
                var adjh = Height * scale * multiplier[0];
                var ttlb = Title * scale;
                var note = adjw - (Notes * scale);
                
                XYZ p1 = new XYZ(0,0,0);
                XYZ p2 = new XYZ(adjw, 0, 0);
                XYZ p3 = new XYZ(0, adjh, 0);
                XYZ p4 = new XYZ(adjw, adjh, 0);
                
                var p5 = new XYZ(0, ttlb, 0);
                var p6 = new XYZ(adjw, ttlb, 0);
                var p7 = new XYZ(note, ttlb, 0);
                var p8 = new XYZ(note, adjh, 0);
                
                var cut = new XYZ(0, 0, 1);
                using(TransactionGroup tg = new TransactionGroup(doc, "Create Reference Planes"))
                {
                    tg.Start();
					using(Transaction t = new Transaction(doc, "Purge RPs"))
					{
						t.Start();
						var rps = new FilteredElementCollector(doc, view.Id).OfCategory(BuiltInCategory.OST_ReferencePlanes).ToList();
						foreach(var rp in rps)
						{
							if(rp.Category.Name == "View Outline")
							{
								doc.Delete(rp.Id);
							}
						}
						t.Commit();
					}
                    using(Transaction t = new Transaction(doc, "Create Category"))
                    {
                        t.Start();
                        subcat = CategorySetup(doc);
                        t.Commit();
                    }
                    using(Transaction t = new Transaction(doc, "Create Zeros"))
                    {
                        t.Start();
                        
                        var rp1 = doc.Create.NewReferencePlane(p1, p2, cut, view);
                        var rp2 = doc.Create.NewReferencePlane(p1, p3, cut, view);
                        var rp3 = doc.Create.NewReferencePlane(p3, p4, cut, view);
                        var rp4 = doc.Create.NewReferencePlane(p2, p4, cut, view);
                        var rp5 = doc.Create.NewReferencePlane(p5, p6, cut, view);
                        var rp6 = doc.Create.NewReferencePlane(p7, p8, cut, view);
                        
                        rp1.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp2.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp3.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp4.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp5.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp6.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                    
                        t.Commit();
                    }
                    tg.Commit();
                }
            }
            else { TaskDialog.Show("Error", "Activate a Drafting View Before Use"); }
        }
        private ElementId CategorySetup(Document doc)
        {
            string name = "View Outline";
            
            var parent = Category.GetCategory(doc, BuiltInCategories.OST_ReferencePlanes);
            CategoryNameMap map = parent.SubCategories;
            var subcat = !map.Contains(name) ? doc.Settings.Categories.NewSubCategory(parent, name) : map.get_Item(name);
            subcat.SetLineWeight(1, GraphicsStyleType.Projection);
            subcat.LineColor = new Color(255, 128, 0);
            subcat.SetLinePatternId(GetDash, GraphicsStyleType.Projection);
			return subcat.Id;
        }
        private static ElementId GetDash(this Document doc)
        {
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(LinePatternElement))
                .ToElementIds().ToList();
            foreach(var id in collector)
            {
                LinePatternElement lpe = doc.GetElement(id) as LinePatternElement;
                string name = lpe.Name;
                if (lpe.Name == "Dash")
                {
                    return lpe.Id;
                }
            }
            LinePattern lp = new LinePattern("Dash");
            
            List<LinePatternSegment> Segments = new List<LinePatternSegment>();
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.05));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.05));
            lp.SetSegments(Segments);
            
            LinePatternElement lped = LinePatternElement.Create(doc, lp);
            return lped.Id;
        }
    }
}

/*
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace GetZero
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("A71EA753-514B-4ED7-8F1F-9A37D93C1FC2")]
	public partial class ThisApplication
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
		public void GetZeros()
		{
			CreateBox();
		}
        private const double Width = 7.375 / 12;
        private const double Height = 7.25 / 12;
        private const double Title = 1.25 / 12;
        private const double Notes = 2.125 / 12;
        
        public void CreateBox()
        {
            var doc = this.ActiveUIDocument.Document;
            var view = doc.ActiveView;
            
            if(view.ViewType == ViewType.DraftingView)
            {
                var scale = view.Scale;
                var multiplier = new int[2] {1,1};
                var adjw = Width * scale * multiplier[1];
                var adjh = Height * scale * multiplier[0];
                var ttlb = Title * scale;
                var note = adjw - (Notes * scale);
                
                XYZ p1 = new XYZ(0,0,0);
                XYZ p2 = new XYZ(adjw, 0, 0);
                XYZ p3 = new XYZ(0, adjh, 0);
                XYZ p4 = new XYZ(adjw, adjh, 0);
                
                var p5 = new XYZ(0, ttlb, 0);
                var p6 = new XYZ(adjw, ttlb, 0);
                var p7 = new XYZ(note, ttlb, 0);
                var p8 = new XYZ(note, adjh, 0);
                
                var cut = new XYZ(0, 0, 1);
                ElementId subcat;
                using(TransactionGroup tg = new TransactionGroup(doc, "Create Reference Planes"))
                {
                    tg.Start();
                    using(Transaction t = new Transaction(doc, "Create Category"))
                    {
                        t.Start();
                        subcat = CategorySetup(doc);
                        t.Commit();
                    }
                    using(Transaction t = new Transaction(doc, "Create Zeros"))
                    {
                        t.Start();
                        
                        var rp1 = doc.Create.NewReferencePlane(p1, p2, cut, view);
                        var rp2 = doc.Create.NewReferencePlane(p1, p3, cut, view);
                        var rp3 = doc.Create.NewReferencePlane(p3, p4, cut, view);
                        var rp4 = doc.Create.NewReferencePlane(p2, p4, cut, view);
                        var rp5 = doc.Create.NewReferencePlane(p5, p6, cut, view);
                        var rp6 = doc.Create.NewReferencePlane(p7, p8, cut, view);
                        
                        rp1.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp2.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp3.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp4.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp5.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        rp6.get_Parameter(BuiltInParameter.CLINE_SUBCATEGORY).Set(subcat);
                        
                        t.Commit();
                    }
                    tg.Commit();
                }
            }
            else { TaskDialog.Show("Error", "Activate a Drafting View Before Use"); }
        }
        private ElementId CategorySetup(Document doc)
        {
            string name = "View Outline";
            
            var parent = Category.GetCategory(doc, BuiltInCategory.OST_CLines);
            CategoryNameMap map = parent.SubCategories;
            var subcat = !map.Contains(name) ? doc.Settings.Categories.NewSubcategory(parent, name) : map.get_Item(name);
            subcat.SetLineWeight(1, GraphicsStyleType.Projection);
            subcat.LineColor = new Color(255, 128, 0);
            subcat.SetLinePatternId(GetDash(doc), GraphicsStyleType.Projection);
            
            return subcat.Id;
        }
        private static ElementId GetDash(Document doc)
        {
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(LinePatternElement))
                .ToElementIds().ToList();
            foreach(var id in collector)
            {
                LinePatternElement lpe = doc.GetElement(id) as LinePatternElement;
                string name = lpe.Name;
                if (lpe.Name == "Dash")
                {
                    return lpe.Id;
                }
            }
            LinePattern lp = new LinePattern("Dash");
            
            List<LinePatternSegment> Segments = new List<LinePatternSegment>();
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.05));
            Segments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.05));
            lp.SetSegments(Segments);
            
            LinePatternElement lped = LinePatternElement.Create(doc, lp);
            return lped.Id;
        }
    }
}
*/
