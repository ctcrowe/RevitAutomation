using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

namespace CC_Plugin
{
    class IDUpdater
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(Execute);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(Execute);
            return Result.Succeeded;
        }
        public static void Execute(object sender, ViewActivatedEventArgs args)
        {
            Document doc = args.Document;
            string id;
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
                    using (Transaction t = new Transaction(doc, "Add Fam Name"))
                    {
                        t.Start();
                        FamNameParam.Add(doc);
                        t.Commit();
                    }
                    using (Transaction t = new Transaction(doc, "Add MasterFormat Parameter"))
                    {
                        t.Start();
                        MFParam.Add(doc);
                        t.Commit();
                    }
                }
                using (Transaction t = new Transaction(doc, "Add ID"))
                {
                    t.Start();
                    if (string.IsNullOrEmpty(IDParam.Get(doc)))
                        id = IDParam.Set(doc);
                    t.Commit();
                }
                tg.Commit();
            }
        }
    }
}
