using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;
using CC_Library;

namespace CC_Plugin
{
    internal class DocSavingAs
    {
        public static void runtest(string s)
        {
            TaskDialog.Show("TEST", s);
        }
        public static void Event(object sender, DocumentSavingAsEventArgs args)
        {
            Document doc = args.Document;
            using (TransactionGroup tg = new TransactionGroup(doc, "Saving Transactions"))
            {
                tg.Start();
                using (Transaction t = new Transaction(doc, "Set ID"))
                {
                    t.Start();
                    IDParam.Set(doc);
                    t.Commit();
                }
                using (Transaction t = new Transaction(doc, "Set Fam Name"))
                {
                    t.Start();
                    FamParam.Set(doc, args.PathName.Split('.').First().Split('\\').Last());
                    t.Commit();
                }
                if (doc.IsFamilyDocument)
                {
                    using (Transaction t = new Transaction(doc, "Set MF Param"))
                    {
                        t.Start();
                        WriteOutput wo = new WriteOutput(runtest);
                        MFParam.Set(doc, GenPredictions.Run(args.PathName.Split('.').First().Split('\\').Last(), wo));
                        t.Commit();
                    }
                }
                tg.Commit();
            }
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSavingAs += new EventHandler<DocumentSavingAsEventArgs>(Event);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSavingAs -= new EventHandler<DocumentSavingAsEventArgs>(Event);
            return Result.Succeeded;
        }
    }
}
