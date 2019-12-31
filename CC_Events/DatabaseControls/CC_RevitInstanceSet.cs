using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using CC_CoreData;
using CC_Parameters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CC_Events
{
    public class RevitInstanceSet
    {
        public List<Param> ParamSet { get; }
        public int Category { get; }
        public bool IsInSet(Param p)
        {
            if(p.Category == Category && p.Tracked)
               return true;
            return false;
        }
        public void GenerateParamSet( Param p)
        {
           ParamSet.Add(p); 
        }
        public RevitInstanceSet(int category)
        {
            ParamSet = new List<Couplet>();
            This.Category = category;
            ParamCheck pc = new ParamCheck(GenerateParamSet);
            ParamQualifier pq = new ParamQualifier(IsInSet);
            ParameterCalls.ParamLoop(pc, pq);
        }
        public CC_CoreData.Instance GenInstance(Element ele)
        {
            List<Couplet> values = new List<Couplet>();
            foreach(Param p in ParamSet)
            {
               values.Add(ParamEdits.GetParameterValue(p, ele);
            }
            return new CC_CoreData.Instance(values.ToArray());
        }
        public static CC_CoreData.Instance[] GenerateInstances(List<Element> eles, int Category)
        {
            List<CC_CoreData.Instance> instances = new List<CC_CoreData.Instance>();
            RevitInstanceSet set = new RevitInstanceSet(Category);
            foreach(Element ele in eles)
            {
                instances.Add(set.GetInstance(ele));
            }
            return instances.ToArray();
        }
        public static void SetEleParamValues(Element ele, Instance inst)
        {
            foreach(Couplet couplet in inst.Values)
            {
                if(ele.get_Parameter(couplet.Param.ID) != null)
                {
                    UpdateParameter(ele, couplet);
                }
            }
        }
    }
}