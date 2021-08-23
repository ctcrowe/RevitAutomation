using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;

using CC_Library;
using CC_Library.Predictions;
using CC_Library.Parameters;

using CC_Plugin.Parameters;

namespace CC_Plugin
{
    internal class SetMF : IUpdater
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSavingAs += new EventHandler<DocumentSavingAsEventArgs>(SavingAsEvent);
            app.ControlledApplication.DocumentSaving += new EventHandler<DocumentSavingEventArgs>(SavingEvent);
            app.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(OpenedEvent);
            app.ControlledApplication.DocumentClosing += new EventHandler<DocumentClosingEventArgs>(ClosingEvent);
            app.ControlledApplication.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(CreatedEvent);
            RegisterUpdater(app.ActiveAddInId);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSavingAs -= new EventHandler<DocumentSavingAsEventArgs>(SavingAsEvent);
            app.ControlledApplication.DocumentSaving -= new EventHandler<DocumentSavingEventArgs>(SavingEvent);
            app.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(OpenedEvent);
            app.ControlledApplication.DocumentClosing -= new EventHandler<DocumentClosingEventArgs>(ClosingEvent);
            app.ControlledApplication.DocumentCreated -= new EventHandler<DocumentCreatedEventArgs>(CreatedEvent);
            SetMF updater = new SetMF(app.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }
        public static void OpenedEvent(object sender, DocumentOpenedEventArgs args)
        {
            Document doc = args.Document;
            if(!doc.IsFamilyDocument)
            {
                ProjectStartup(doc);
            }
            UpdateMFDB.ProjectStartup(args.Document.Application.ActiveAddInId, args.Document);
        }
        public static void ClosingEvent(object sender, DocumentClosingEventArgs args)
        {
            Document doc = args.Document;
            UpdateMFDB.ProjectShutdown(args.Document.Application.ActiveAddInId, args.Document);
        }
        public static void CreatedEvent(object sender, DocumentCreatedEventArgs args)
        {
            Document doc = args.Document;
            if (!doc.IsFamilyDocument)
            {
                ProjectStartup(doc);
            }
        }
        public static void SavingAsEvent(object sender, DocumentSavingAsEventArgs args)
        {
            Document doc = args.Document;
            if (doc.IsFamilyDocument)
            {
                string fn = args.PathName.Split('\\').Last().Split('.').First();
                SetFamily(fn, doc);
            }
        }
        public static void SavingEvent(object sender, DocumentSavingEventArgs args)
        {
            Document doc = args.Document;
            if (doc.IsFamilyDocument)
            {
                string fn = doc.PathName.Split('\\').Last().Split('.').First();
                SetFamily(fn, doc);
            }
        }
        private static void RegisterUpdater(AddInId id)
        {
            ElementId FamName = new ElementId(BuiltInParameter.SYMBOL_NAME_PARAM);
            SetMF updater = new SetMF(id);
            UpdaterRegistry.RegisterUpdater(updater, true);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(FamilySymbol)),
                Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(FamilyInstance)),
                Element.GetChangeTypeElementAddition());
        }
        public static void ProjectStartup(Document doc)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Preupdater Registration"))
            {
                tg.Start();
                using (Transaction t = new Transaction(doc, "Add MF Param"))
                {
                    t.Start();
                    try { doc.AddParam(Params.Masterformat); } catch (Exception e) { e.OutputError(); }
                    t.Commit();
                }
                tg.Commit();
            }
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            if (!doc.IsFamilyDocument)
            {
                var Eles = data.GetModifiedElementIds().Concat(data.GetAddedElementIds()).ToList();
                foreach (var eid in Eles)
                {
                    var ele = doc.GetElement(eid) as FamilySymbol;
                    if (ele != null)
                    {
                        string name = "";
                        int MF = 0;
                        try { name = ele.FamilyName + " " + ele.Name; } catch (Exception e) { e.OutputError(); }
                        if (name != "")
                        {
                            try
                            {
                                Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                                s.TextInput = name;
                                var output = new MasterformatNetwork().Predict(s);
                                MF = output.ToList().IndexOf(output.Max());
                            }
                            catch (Exception e) { e.OutputError(); }
                        }
                        try { ele.Set(Params.Masterformat, MF.ToString()); } catch (Exception e) { e.OutputError(); }
                    }
                    else
                    {
                        var inst = doc.GetElement(eid) as FamilyInstance;
                        if(inst != null)
                        {
                            ele = inst.Symbol;
                            string name = "";
                            int MF = 0;
                            try { name = ele.FamilyName + " " + ele.Name; } catch (Exception e) { e.OutputError(); }
                            if (name != "")
                            {
                                try
                                {
                                    Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                                    s.TextInput = name;
                                    var output = new MasterformatNetwork().Predict(s);
                                    MF = output.ToList().IndexOf(output.Max());
                                }
                                catch (Exception e) { e.OutputError(); }
                            }
                            try { ele.Set(Params.Masterformat, MF.ToString()); } catch (Exception e) { e.OutputError(); }
                        }
                    }
                }
            }
        }
        public static void SetFamily(string fn, Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                s.TextInput = fn;
                var output = new MasterformatNetwork().Predict(s);
                var Masterformat = output.ToList().IndexOf(output.Max());
                using (TransactionGroup tg = new TransactionGroup(doc, "Set MF"))
                {
                    tg.Start();
                    using (Transaction t = new Transaction(doc, "Add MF Param"))
                    {
                        t.Start();
                        try { doc.AddParam(Params.Masterformat); }
                        catch (Exception e) { e.OutputError(); }
                        t.Commit();
                    }
                    if (doc.FamilyManager.Types.Size < 1)
                    {
                        using (Transaction t = new Transaction(doc, "Create Family Type"))
                        {
                            t.Start();
                            doc.FamilyManager.NewType("Automatic Type");
                            t.Commit();
                        }
                    }
                    using (Transaction t = new Transaction(doc, "Set MF Param"))
                    {
                        t.Start();
                        try { doc.SetFamilyParam(Params.Masterformat, Masterformat.ToString()); }
                        catch (Exception e) { e.OutputError(); }
                        t.Commit();
                    }
                    tg.Commit();
                }
            }
        }
        public SetMF(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("fcff7854-c912-4f24-afad-0ea4872c399e"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Updates MF Section Based on Family Name and Type Name"; }
        public ChangePriority GetChangePriority() { return ChangePriority.DetailComponents; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update MF Division"; }
    }
}
