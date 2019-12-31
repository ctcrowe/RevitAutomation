using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using CC_CoreData;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CC_Events
{
    internal class ElementDatabase
    {
        public static void SetParameters(Element ele, CC_CoreData.Instance inst)
        {
            for (int i = 0; i < inst.Values.Count(); i++)
            {
                //ParamEdits.UpdateParameter(ele, inst.Values[i]);
            }
            Parameter ToFileParam = ele.get_Parameter(ParameterLibrary.ToFile.ID);
            ToFileParam.Set(1);
        }
        public static CC_CoreData.Instance[] GetInstances(List<Element> ElementList, Param[] Paramset)
        {
            Document currentDoc = ElementList.FirstOrDefault().Document;
            ProjectInfo info = currentDoc.ProjectInformation;
            List<CC_CoreData.Instance> instances = new List<CC_CoreData.Instance>();

            for (int i = 0; i < ElementList.Count(); i++)
            {
                FamilySymbol symb = ElementList[i] as FamilySymbol;
                Family fam = symb.Family;
                if (ElementList[i].get_Parameter(ParameterLibrary.ToFile.ID) != null)
                {
                    Parameter inclparam = ElementList[i].get_Parameter(ParameterLibrary.ToFile.ID);
                    if (inclparam.AsInteger() == 1)
                    {
                        Couplet[] pairs = new Couplet[Paramset.Count()];
                        for (int j = 0; j < pairs.Count(); j++)
                        {
                            if (ElementList[i].get_Parameter(Paramset[j].ID) != null)
                            {
                                //Couplet pair = ParamEdits.GetParameterValue(Paramset[j], ElementList[i]);
                                //pairs[j] = pair;
                            }
                            else
                            {
                                Couplet pair = new Couplet(Paramset[j], "");
                                pairs[j] = pair;
                            }
                        }
                        CC_CoreData.Instance e = new CC_CoreData.Instance(pairs);
                        instances.Add(e);
                    }
                }
            }
            return instances.ToArray();
        }
        public static void ReadFile(string filename, List<Element> ElementList, CC_CoreData.Instance[] instances)
        {
            if (!File.Exists(DBIdentifiers.SharedParams))
            {
                using (FileStream stream = File.Create(DBIdentifiers.SharedParams))
                {
                    stream.Close();
                }
            }
            Document currentDoc = ElementList.FirstOrDefault().Document;
            Application App = currentDoc.Application;
            App.SharedParametersFilename = DBIdentifiers.SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();

            for (int i = 0; i < instances.Count(); i++)
            {
                if (instances[i].Values[1].Value.EndsWith(".rfa"))
                {
                    currentDoc.LoadFamily(instances[i].Values[1].Value);
                }
                if (ElementList.Any(x => x.get_Parameter(instances[i].Values[0].Param.ID).AsString() == instances[i].Values[0].Value))
                {
                    ElementDatabase.SetParameters(ElementList.Where(x =>
                    x.get_Parameter(instances[i].Values[0].Param.ID).AsString()
                    == instances[i].Values[0].Value).First(), instances[i]);
                }
            }
        }
    }
}