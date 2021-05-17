using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;

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
            app.ControlledApplication.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(CreatedEvent);
            RegisterUpdater(app.ActiveAddInId);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSavingAs -= new EventHandler<DocumentSavingAsEventArgs>(SavingAsEvent);
            app.ControlledApplication.DocumentSaving -= new EventHandler<DocumentSavingEventArgs>(SavingEvent);
            app.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(OpenedEvent);
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
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_GenericModel), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_Doors), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_CurtainWallPanels), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_Windows), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_Casework), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_DetailComponents), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_Furniture), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_FurnitureSystems), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_SpecialityEquipment), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_MechanicalEquipment), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_LightingFixtures), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_ElectricalEquipment), Element.GetChangeTypeParameter(FamName));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementCategoryFilter(BuiltInCategory.OST_PlumbingFixtures), Element.GetChangeTypeParameter(FamName));
        }
        public static void ProjectStartup(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                using (TransactionGroup tg = new TransactionGroup(doc, "Preupdater Registration"))
                {
                    tg.Start();
                    using (Transaction t = new Transaction(doc, "Add MF Param"))
                    {
                        t.Start();
                        try { doc.AddParam(Params.Masterformat); } catch { }
                        t.Commit();
                    }
                    tg.Commit();
                }
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
                        try { name = ele.FamilyName + " " + ele.Name; } catch { }
                        if (name != "") { MF = MasterformatNetwork.Predict(name); }
                        try { ele.SetElementParam(Params.Masterformat, MF.ToString()); } catch { }
                    }
                }
            }
        }
        public static void SetFamily(string fn, Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                var Masterformat = MasterformatNetwork.Predict(fn);
                using (TransactionGroup tg = new TransactionGroup(doc, "Set MF"))
                {
                    tg.Start();
                    using (Transaction t = new Transaction(doc, "Add MF Param"))
                    {
                        t.Start();
                        try { doc.AddParam(Params.Masterformat); } catch { }
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
                        doc.SetFamilyParam(Params.Masterformat, Masterformat.ToString());
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
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Update MF Division"; }
    }
}
