using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB;

namespace CC_Plugin
{
    internal class DocSynching
    {
        public static void synch(DocumentSynchronizingWithCentralEventArgs args)
        {
            Document doc = args.Document;
            using (Transaction t = new Transaction(doc, "Add Families"))
            {
                t.Start();
                EmbeddedFamilies.run(doc, "Symbols");
                t.Commit();
            }
        }
    }
}
