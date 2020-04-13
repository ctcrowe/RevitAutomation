using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal class ViewChanged
    {
        private static void Execute(object sender, ViewActivatedEventArgs args)
        {
            Document doc = args.Document;
            using (TransactionGroup tg = new TransactionGroup(doc, "Add ID Group"))
            {
                tg.Start();
                if (doc.IsFamilyDocument)
                {
                    if (doc.FamilyManager.Types.Size < 1)
                    {
                        using (Transaction t = new Transaction(doc, "Create Family Type"))
                        {
                            t.Start();
                            doc.FamilyManager.NewType("Automatic Type");
                            t.Commit();
                        }
                    }
                    using (Transaction t = new Transaction(doc, "Add Categories"))
                    {
                        t.Start();
                        doc.AddCategories();
                        t.Commit();
                    }
                }
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
                tg.Commit();
            }
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(Execute); } catch {}
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(Execute);
            return Result.Succeeded;
        }
    }
}