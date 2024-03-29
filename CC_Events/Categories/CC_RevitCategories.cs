﻿using System;
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
                    t = typeof(ObjectStyles_Casework);
                    break;
                case (int)BuiltInCategory.OST_Doors:
                    t = typeof(ObjectStyles_Doors);
                    break;
            }
        
            if(t != null)
            {
                using (TransactionGroup tg = new TransactionGroup(doc, "ChangeCategories"))
                {
                    tg.Start();
                    foreach(string tn in Enum.GetNames(t))
                    {
                        using (Transaction trans = new Transaction(doc, tn))
                        {
                            trans.Start();
                            Category Sub = cat.SubCategories.Contains(tn) ? cat.SubCategories.get_Item(tn) : doc.Settings.Categories.NewSubcategory(cat, tn);
                            trans.Commit();
                        }
                    }

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
                        if(!Enum.GetNames(t).Contains(sc.Name))
                        {
                            using(Transaction trans = new Transaction(doc, sc.Name))
                            {
                                trans.Start();
                                try { doc.Delete(sc.Id); } catch { }
                                trans.Commit();
                            }
                        }
                        else
                        {
                            using(Transaction trans = new Transaction(doc, sc.Name))
                            {
                                trans.Start();
                                sc.SetProjectedLineWeight();
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
