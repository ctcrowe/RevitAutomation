using System;
using Autodesk.Revit.DB;
using CC_CoreData;

namespace CC_Events
{
    class FamilyGUIDReg
    {
        public static void GUIDReg(Document doc, DefinitionFile deffile)
        {
            ParamFileSetup pfs = new ParamFileSetup(deffile);

            pfs.PCheck(ParameterLibrary.FamilyID);
            
            if (doc.IsFamilyDocument)
            {
                FamilyManager mgr = doc.FamilyManager;
                if (mgr.get_Parameter(ParameterLibrary.FamilyID.ID) == null)
                {
                    using (TransactionGroup tg = new TransactionGroup(doc, "GUID Setup"))
                    {
                        string id = Guid.NewGuid().ToString("N");
                        tg.Start();
                        using (Transaction t1 = new Transaction(doc, "Add GUID"))
                        {
                            t1.Start();
                            RevitParamSetup.AddParam(doc, ParameterLibrary.FamilyID);
                            t1.Commit();
                        }
                        ChangeFamID cid = new ChangeFamID(doc);
                        tg.Commit();
                    }
                }
            }
        }
    }
}