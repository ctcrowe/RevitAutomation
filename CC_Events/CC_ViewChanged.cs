using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace CC_Plugin
{
    class ViewChanged
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
                }
                CommandLibrary.Transact(new CommandLibrary.DocStringCommand(IDParam.Set), doc);
                tg.Commit();
            }
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(FamParam.Add); } catch {}
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(MFParam.Add); } catch {}
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(MFConfirmParam.Add); } catch {}
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(WidthParam); } catch {}
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(HeightParam); } catch {}
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(DepthParam); } catch {}
            try { app.ViewActivated += new EventHandler<ViewActivatedEventArgs>(Execute); } catch {}
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(FamParam.Add);
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(MFParam.Add);
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(MFConfirmParam.Add);
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(WidthParam);
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(HeightParam);
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(DepthParam);
            app.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(Execute);
            return Result.Succeeded;
        }
    }
}
