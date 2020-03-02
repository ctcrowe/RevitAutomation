﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CC_Library.Parameters;

namespace CC_Plugin
{
    class ViewChanged
    {
        public static void AddParams(Document doc, IDParam id)
        {
            Foreach(var p in ...)
            {
                using(Transaction t = new Transaction(doc, "Add Params"))
                {
                    t.Start();
                    try
                    {
                    switch(p.Location)
                    {
                        case ParamLocation....
                            break;
                        case ParanLocation....
                            break;
                        case ParamLocation....
                            break;
                        case ParamLocation....
                            break;
                    }
                    t.Commit();
                    }
                    catch { t.RollBack(); }
                }
            }
        }
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
                }
                Transactions.Run(new IDParam().AddComboParam(doc), doc);
                IDParam id = new IDParam();
                AddParams(doc, id);
                using (Transaction t = new Transaction(doc, "Set ID"))
                {
                    t.Start();
                    IDParam.Set(doc);
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
