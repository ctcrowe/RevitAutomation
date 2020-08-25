using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using CC_RevitBasics;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class AddParameters
    {
        public static void AddParams(this Document doc)
        {
            ResetParamLibrary.Run();
            using (TransactionGroup tg = new TransactionGroup(doc, "Doc Synching"))
            {
                tg.Start();
                foreach (CCParameter p in Enum.GetValues(typeof(CCParameter)))
                {
                    using (Transaction t = new Transaction(doc, "ADD Parameters"))
                    {
                        t.Start();
                        doc.AddParam(p);
                        t.Commit();
                    }
                }
                tg.Commit();
            }
        }
    }
}
