using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace CC_Events
{
    class CheckNameUse
    {
        public static string Check(Document doc, string name)
        {
            bool FirstNameUsed = false;
            int j = 0;
            FilteredElementCollector SymbCollector = new FilteredElementCollector(doc);
            List<Element> symbols = SymbCollector.OfClass(typeof(FamilySymbol)).ToList();

            while(true)
            {
                j++;
                bool IsUsed = false;
                foreach (Element ele in symbols)
                {
                    FamilySymbol famsymb = ele as FamilySymbol;
                    if(j < 2 && famsymb.Family.Name == name)
                    {
                        FirstNameUsed = true;
                        IsUsed = true;
                        break;
                    }
                    if (famsymb.Family.Name == name + j)
                    {
                        IsUsed = true;
                        break;
                    }
                }
                if (!IsUsed)
                    break;
            }
            if (!FirstNameUsed)
                return name;
            return name + "_" + j;
        }
    }
}