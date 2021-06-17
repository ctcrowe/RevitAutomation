using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CC_Library.Parameters;
using System.Threading;
using CC_Plugin.Parameters;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;

using Autodesk.Revit.DB.Architecture;
using CC_Library.Predictions;
using CC_Library;
/*
This isnt going to work. Abandoned

namespace CC_Plugin
{
    internal class FamUpdateMFDB : IUpdater
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(Event);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(Event);
            return Result.Succeeded;
        }
        public static void Event(object sender, DocumentChangedEventArgs args)
        {
            Document doc = args.GetDocument();
            if (doc.IsFamilyDocument)
            {
                var ids = args.GetModifiedElementIds();
                var pid = doc.FamilyManager.get_Parameter(Params.Masterformat.Guid).Id;
                if (ids.Any(x => x.IntegerValue == pid.IntegerValue))
                {
                    try
                    {
                        TaskDialog.Show("Test", "The Masterformat Parameter was changed!");
                    }
                    catch (Exception e)
                    {
                        e.OutputError();
                    }
                }
            }
        }
        public static void ProjectStartup(AddInId id, Document doc)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Preupdater Registration"))
            {
                tg.Start();
                if (doc.IsFamilyDocument)
                {
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
                            try
                            {
                                t.Start();
                                doc.FamilyManager.NewType("Automatic Type");
                                t.Commit();
                            }
                            catch (Exception e) { e.OutputError(); }
                        }
                    }
                    try
                    {
                        FamilyParameter p = doc.FamilyManager.get_Parameter(Params.Masterformat.Guid);

                        FamUpdateMFDB updater = new FamUpdateMFDB(id);
                        UpdaterRegistry.RegisterUpdater(updater, doc, true);
                        ElementClassFilter symbs = new ElementClassFilter(typeof(FamilyType));
                        UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), symbs, Element.GetChangeTypeParameter(p.Id));
                    }
                    catch (Exception e) { e.OutputError(); }
                }
                tg.Commit();
            }
        }
        public static void ProjectShutdown(AddInId id, Document doc)
        {
            FamUpdateMFDB updater = new FamUpdateMFDB(id);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId(), doc);
        }
        public void Execute(UpdaterData data)
        {
            TaskDialog.Show("Test", "The Updater Triggered");
            /*
            try
            {
                Document doc = data.GetDocument();
                if (doc.IsFamilyDocument)
                {
                    var Eles = data.GetModifiedElementIds().ToList();
                    foreach (var eid in Eles)
                    {
                        string name = string.Empty;
                        try { name = ele.FamilyName + " " + ele.Name; } catch { }
                        var Actual = "";
                        var type = doc.FamilyManager.CurrentType;
                        var Prediction = MasterformatNetwork.Predict(name).ToString();

                        if (Prediction != Actual)
                        {
                            List<string> LineList = new List<string>();
                            string Folder = "NeuralNets".GetMyDocs();
                            if (!Directory.Exists(Folder))
                                Directory.CreateDirectory(Folder);
                            string filepath = Folder + "\\MasterformatData.txt";
                            if (File.Exists(filepath))
                                LineList = File.ReadAllLines(filepath).ToList();
                            if (!string.IsNullOrEmpty(name)) LineList.Add("Masterformat," + name + "," + Actual);
                            if (LineList.Count() == 100)
                                LineList.RemoveAt(0);

                            File.WriteAllLines(filepath, LineList);
                            try
                            {
                                MasterformatNetwork.ActivePropogate(LineList);
                            }
                            catch (Exception e)
                            {
                                e.OutputError();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            { e.OutputError(); }
            
        }
        public FamUpdateMFDB(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("9b29c19e-ed47-41f7-8d78-73bd262db077"));
        }

        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Modifies MF Data based on input in families"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Updates MF Division Database from changes in Families"; }
    }
}*/
