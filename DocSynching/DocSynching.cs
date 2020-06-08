using System;
using Autodesk.Revit.DB;
using CC_Library.Parameters;

using CC_RevitBasics;

namespace CC_DocSynching
{
    internal class DocSynching
    {
        public static void synch(Document doc)
        {
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
                using (Transaction t = new Transaction(doc, "Set ID"))
                {
                    t.Start();
                    doc.SetID(doc.CheckID());
                    t.Commit();
                }
                using (Transaction t = new Transaction(doc, "Predict Room Privacy"))
                {
                    t.Start();
                    View v = doc.ActiveView;
                    v.UpdateRoomPrivacy();
                    t.Commit();
                }
                using (Transaction t = new Transaction(doc, "Collect Keynotes"))
                {
                    doc.CollectKeynotes();
                }
                /*doc.PurgeCategories();*/
                tg.Commit();
            }
        }
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string dir = directory + "\\CC_PrjData";
    }
}