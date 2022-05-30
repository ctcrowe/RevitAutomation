using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using CC_Library;
using CC_Library.Predictions;

namespace CC_Plugin
{
    class RevitCategories
    {
        public static void ReviseCategories(Document doc)
        {
            Category cat = doc.OwnerFamily.FamilyCategory;
            Type t = null;
        
            switch(cat.Id.IntegerValue)
            {
                default:
                    break;
                case (int)BuiltInCategory.OST_Casework:
                    t = typeof(CC_Library.ObjectStyles_Casework);
                    break;
            }
        
            if(t != null)
            {
                using (TransactionGroup tg = new TransactionGroup(doc, "ChangeCategories"))
                {
                    tg.Start();

                    List<string> Subcats = new List<string>();

                    var forms = new FilteredElementCollector(doc).OfClass(typeof(GenericForm)).ToElementIds().ToList();
                    foreach (ElementId e in forms)
                    {
                        GenericForm ele = doc.GetElement(e) as GenericForm;
                        if (ele != null)
                        {
                            string subcat;

                            if (ele.Subcategory == null) { subcat = Enum.GetNames(t)[0]; }
                            else
                            {
                                var name = ele.Subcategory.Name;
                                var numb = ObjStyleNetwork.Predict(name, t, new WriteToCMDLine(CMDLibrary.WriteNull));
                                subcat = Enum.GetNames(t)[numb.ToList().IndexOf(numb.Max())];
                                t.CreateEmbed(name, subcat);
                            }

                            if(!Subcats.Contains(subcat))
                                Subcats.Add(subcat);

                            using (Transaction trans = new Transaction(doc, "set Category"))
                            {
                                trans.Start();
                                Category Sub = cat.SubCategories.Contains(subcat) ? cat.SubCategories.get_Item(subcat) : doc.Settings.Categories.NewSubcategory(cat, subcat);
                                ele.Subcategory = Sub;
                                trans.Commit();
                            }
                        }
                    }
                    var map = cat.SubCategories;
                    var iter = map.ForwardIterator();
                    while(iter.MoveNext())
                    {
                        var sc = iter.Current as Category;
                        if(!Subcats.Contains(sc.Name))
                        {
                            using(Transaction trans = new Transaction(doc, sc.Name))
                            {
                                trans.Start();
                                try { doc.Delete(sc.Id); } catch { }
                                trans.Commit();
                            }
                        }
                    }
                    
                    tg.Commit();
                }
            }
        }
    }
}