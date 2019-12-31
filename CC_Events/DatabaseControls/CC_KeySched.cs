using CC_CoreData;

using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

namespace CC_Events
{
    public class KeySchedule
    {
        private static CC_CoreData.Instance GetParamValues(Element e, Param[] paramset)
        {
            Couplet[] vals = new Couplet[paramset.Count()];
            vals[0] = new Couplet(paramset[0], e.get_Parameter(BuiltInParameter.REF_TABLE_ELEM_NAME).AsString());
            foreach(Parameter p in e.Parameters)
            {
                if(paramset.Any(x => x.ShortName == p.Definition.Name.ToUpper()))
                {
                    vals[Array.IndexOf(paramset, paramset
                        .Where(x => x.ShortName == p.Definition.Name.ToUpper()).First())] = 
                        new Couplet(paramset.Where(x => x.ShortName == p.Definition.Name.ToUpper()).First(), p.AsString());
                }
                else
                {
                    if(paramset.Any(x => x.AlternateNames.Any(y => y == p.Definition.Name.ToUpper())))
                    {
                        vals[Array.IndexOf(paramset, paramset
                            .Where(x => x.AlternateNames.Any(y => y == p.Definition.Name.ToUpper())).First())] = 
                            new Couplet(paramset.Where(x => x.AlternateNames.Any(y => y == p.Definition.Name.ToUpper())).First(), p.AsString());
                    }
                }
            }
            return new CC_CoreData.Instance(vals);
        }
        public static CC_CoreData.Instance[] GetKeySchedInstances(ViewSchedule v, Param[] p)
        {
            IEnumerable<Element> eles = from e in new FilteredElementCollector(v.Document, v.Id) select e;
            CC_CoreData.Instance[] instances = new CC_CoreData.Instance[eles.Count()];
            int j = 0;
            for(int i = 0; i < eles.Count(); i++)
            {
                instances[j] = GetParamValues(eles.ElementAt(i), p);
                j++;
            }
            return instances;
        }
        public static ViewSchedule GetKeySchedule(Document currentDoc, string[] nameOptions)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(currentDoc);
            List<Element> scheds = viewCollector.OfClass(typeof(ViewSchedule)).ToList();
            for (int i = 0; i < scheds.Count(); i++)
            {
                ViewSchedule vs = scheds[i] as ViewSchedule;
                if (vs.Definition.IsKeySchedule)
                {
                    if (nameOptions.Contains(vs.ViewName.ToUpper()))
                    {
                        return vs;
                    }
                }
            }
            return null;
        }
        private static void SetParamValues(Element e, CC_CoreData.Instance instance, Param[] p, bool AdjustComments)
        {
            foreach(Parameter par in e.Parameters)
            {
                if(par.Definition.Name.ToUpper() == "COMMENTS" && AdjustComments)
                {
                    par.Set(instance.Values[1].Value);
                }
                if(p.Any(x => x.ShortName == par.Definition.Name.ToUpper()))
                {
                    par.Set(instance.Values[Array.IndexOf(p, p.Where(x => x.ShortName == par.Definition.Name.ToUpper()).First())].Value);
                }
                else
                {
                    if(p.Any(x => x.AlternateNames.Any(y => y == par.Definition.Name.ToUpper())))
                    {
                        par.Set(instance.Values[Array.IndexOf(p,
                            p.Where(x => x.AlternateNames.
                            Any(y => y == par.Definition.Name.ToUpper())).First())].Value);
                    }
                }
            }
            e.get_Parameter(BuiltInParameter.REF_TABLE_ELEM_NAME).Set(instance.Values[0].Value);
        }
        private static void CreateInstance(ViewSchedule v)
        {
            TableSectionData body = v.GetTableData().GetSectionData(SectionType.Body);
            body.InsertRow(body.LastRowNumber + 1);
        }
        private static List<Element> CompareSchedules(List<Element> beginningsched, List<Element> finishsched)
        {
            List<Element> newsched = new List<Element>();
            foreach(Element ele in finishsched)
            {
                if(!beginningsched.Any(x => x.Id == ele.Id))
                {
                    newsched.Add(ele);
                }
            }
            return newsched;
        }
        public static void UpdateKeySchedule(ViewSchedule v, CC_CoreData.Instance[] instances, Param[] p, bool AdjustComments)
        {
            IEnumerable<Element> elements = from x in new FilteredElementCollector(v.Document, v.Id) select x;
            using (TransactionGroup tg = new TransactionGroup(v.Document, "Update Schedule"))
            {
                List<CC_CoreData.Instance> UnusedInstances = new List<CC_CoreData.Instance>();
                tg.Start();
                using (Transaction trans1 = new Transaction(v.Document, "Update Existing Elements"))
                {
                    trans1.Start();
                    foreach (CC_CoreData.Instance i in instances)
                    {
                        if (elements.Any(x => x.Name == i.Values[0].Value))
                        {
                            SetParamValues(elements.Where(x => x.Name == i.Values[0].Value).First(), i, p, AdjustComments);
                        }
                        else
                        {
                            UnusedInstances.Add(i);
                        }
                    }
                    trans1.Commit();
                }

                List<Element> scheduleBefore = new FilteredElementCollector(v.Document, v.Id).ToList();
                using (Transaction trans2 = new Transaction(v.Document, "Add New Elements"))
                {
                    trans2.Start();
                    foreach (CC_CoreData.Instance i in UnusedInstances)
                    {
                        CreateInstance(v);
                    }
                    trans2.Commit();
                }
                List<Element> scheduleAfter = new FilteredElementCollector(v.Document, v.Id).ToList();
                List<Element> elestoadd = CompareSchedules(scheduleBefore, scheduleAfter);
                
                using (Transaction trans3 = new Transaction(v.Document, "Update New Elements"))
                {
                    trans3.Start();
                    for(int i = 0; i < elestoadd.Count(); i++)
                    {
                        SetParamValues(elestoadd[i], UnusedInstances[i], p, AdjustComments);
                    }
                    trans3.Commit();
                }
                tg.Assimilate();
            }
        }
    }
}