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
            ResetParamLibrary.Run();

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
                AddRevitParams.AddParams(doc);
                IDParam id = new IDParam();
                id.GetIDParam(doc);
                if (string.IsNullOrEmpty(id.Value))
                {
                    using (Transaction t = new Transaction(doc, "Set ID"))
                    {
                        t.Start();
                        id.SetIDParam(doc);
                        t.Commit();
                    }
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