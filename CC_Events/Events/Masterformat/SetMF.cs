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
    internal class MFElePlaced : IUpdater
    {
        public static void StartUp(AddInId id)
        {
            MFElePlaced updater = new MFElePlaced(id);
            UpdaterRegistry.RegisterUpdater(updater, true);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(FamilyInstance)),
                Element.GetChangeTypeElementAddition());
        }
        public static void ShutDown(AddInId id)
        {
            MFElePlaced updater = new MFElePlaced(id);
            UpdaterRegistry.UnregisterUpdater(updater, true);
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            if (!doc.IsFamilyDocument)
            {
                var Eles = data.GetAddedElementIds().ToList();
                foreach (var eid in Eles)
                {
                    var ele = doc.GetElement(eid) as FamilySymbol;
                    if (ele != null)
                    {
                        var inst = doc.GetElement(eid) as FamilyInstance;
                        if (inst != null)
                        {
                            ele = inst.Symbol;
                            string name = "";
                            int MF = 0;
                            try { name = ele.FamilyName; } catch (Exception e) { e.OutputError(); }
                            if (name != "")
                            {
                                try
                                {
                                    Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                                    s.TextInput = name;
                                    var output = new MasterformatNetwork(s).Predict(s);
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
        public MFElePlaced(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("0517c3c7-3812-4689-9ff8-70b3444cab27"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Updates MF Section Based on Family Name and Type Name"; }
        public ChangePriority GetChangePriority() { return ChangePriority.DetailComponents; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update MF Div on Placement"; }
    }
    internal class MFTypeNameChange : IUpdater
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            MFElePlaced.StartUp(app.ActiveAddInId);
            app.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(OpenedEvent);
            app.ControlledApplication.DocumentClosing += new EventHandler<DocumentClosingEventArgs>(ClosingEvent);
            app.ControlledApplication.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(CreatedEvent);
            RegisterUpdater(app.ActiveAddInId);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            MFElePlaced.ShutDown(app.ActiveAddInId);
            app.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(OpenedEvent);
            app.ControlledApplication.DocumentClosing -= new EventHandler<DocumentClosingEventArgs>(ClosingEvent);
            app.ControlledApplication.DocumentCreated -= new EventHandler<DocumentCreatedEventArgs>(CreatedEvent);
            MFTypeNameChange updater = new MFTypeNameChange(app.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }
        public static void OpenedEvent(object sender, DocumentOpenedEventArgs args)
        {
            Document doc = args.Document;
            if (!doc.IsFamilyDocument)
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
        private static void RegisterUpdater(AddInId id)
        {
            ElementId FamName = new ElementId(BuiltInParameter.SYMBOL_NAME_PARAM);
            MFTypeNameChange updater = new MFTypeNameChange(id);
            UpdaterRegistry.RegisterUpdater(updater, true);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(),
                new ElementClassFilter(typeof(FamilySymbol)),
                Element.GetChangeTypeParameter(FamName));
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
                var Eles = data.GetModifiedElementIds().ToList();
                foreach (var eid in Eles)
                {
                    var ele = doc.GetElement(eid) as FamilySymbol;
                    if (ele != null)
                    {
                        string name = "";
                        int MF = 0;
                        try { name = ele.FamilyName; } catch (Exception e) { e.OutputError(); }
                        if (name != "")
                        {
                            try
                            {
                                Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                                s.TextInput = name;
                                var output = new MasterformatNetwork(s).Predict(s);
                                MF = output.ToList().IndexOf(output.Max());
                            }
                            catch (Exception e) { e.OutputError(); }
                        }
                        try { ele.Set(Params.Masterformat, MF.ToString()); } catch (Exception e) { e.OutputError(); }
                    }
                }
            }
        }
        public MFTypeNameChange(AddInId id)
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
