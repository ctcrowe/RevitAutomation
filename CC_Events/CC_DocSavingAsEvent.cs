using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;

namespace CC_Plugin
{
    internal class DocSavingAs
    {
        public static void Event(object sender, DocumentSavingAsEventArgs args)
        {
            Document doc = args.Document;
            using (Transaction t = new Transaction(doc, "Saving Transaction"))
            {
                t.Start();
                string id = IDParam.Set(doc);
                string Fam = FamParam.Set(doc, args.PathName.Split('.').First().Split('\\').Last());
                t.Commit();
            }
        }
    }
}