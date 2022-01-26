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
                var adjw = Width * Scale * Multiplier[1];
                var adjh = Height * Scale * Multiplier[0];
                var ttlb = Title * Scale;
                var note = adjw - (Notes * Scale);
                
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
                    using(Transaction t = new Transaction(doc, "Create Category"))
                    {
                        t.Start();
                        CategorySetup(doc);
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
                    
                    t.Commit();
                }
                    tg.Commit();
                }
            }
            else { TaskDialog.Show("Error", "Activate a Drafting View Before Use"); }
        }
        private void CategorySetup(Document doc)
        {
            string name = "View Outline";
            
            var parent = Category.GetCategory(doc, BuiltInCategories.OST_ReferencePlanes);
            CategoryNameMap map = parent.SubCategories;
            var subcat = !map.Contains(name) ? doc.Settings.Categories.NewSubCategory(parent, name) : map.get_Item(name);
            setCategoryStyles(subcat, cat);
        }
    }
}
