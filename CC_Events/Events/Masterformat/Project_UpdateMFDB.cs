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

using Autodesk.Revit.DB.Architecture;
using CC_Library.Predictions;
using CC_Library;

namespace CC_Plugin
{
    internal class UpdateMFDB : IUpdater
    {
        public static void ProjectStartup(AddInId id, Document doc)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Preupdater Registration"))
            {
                tg.Start();
                if (!doc.IsFamilyDocument)
                {
                    using (Transaction t = new Transaction(doc, "Add Parameters"))
                    {
                        t.Start();
                        doc.AddParam(Params.Masterformat);
                        t.Commit();
                    }
                    /*
                    FamilySymbol f;
                    using (Transaction t = new Transaction(doc, "Add Room"))
                    {
                        t.Start();
                        f = doc.Create.NewFamilyInstance(doc.Phases.get_Item(0));
                        t.Commit();
                    }*/
                    Element symb = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).FirstOrDefault();
                    Parameter p = symb.get_Parameter(Params.Masterformat.Guid);

                    UpdateMFDB updater = new UpdateMFDB(id);
                    UpdaterRegistry.RegisterUpdater(updater, doc, true);
                    ElementClassFilter symbs = new ElementClassFilter(typeof(FamilySymbol));
                    UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), symbs, Element.GetChangeTypeParameter(p.Id));

                    /*
                    using (Transaction t = new Transaction(doc, "Add Room"))
                    {
                        t.Start();
                        doc.Delete(r.Id);
                        t.Commit();
                    }*/
                }
                tg.Commit();
            }
        }
        public static void ProjectShutdown(AddInId id, Document doc)
        {
            UpdateMFDB updater = new UpdateMFDB(id);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId(), doc);
        }
        private static string Write(string s)
        {
            TaskDialog.Show("Testing Error Change", s);
            return s;
        }
        public void Execute(UpdaterData data)
        {
            try
            {
                Document doc = data.GetDocument();
                if (!doc.IsFamilyDocument)
                {
                    var Eles = data.GetModifiedElementIds().ToList();
                    foreach (var eid in Eles)
                    {
                        var ele = doc.GetElement(eid) as FamilySymbol;
                        string name = string.Empty;
                        try { name = ele.FamilyName + " " + ele.Name; } catch { }
                        var Actual = ele.GetElementParam(Params.Masterformat);
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
        public UpdateMFDB(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("13c9b8ac-04cf-49ba-b9ec-cf87bd78cd55"));
        }

        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return "Modifies MF Data based on input"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Updates MF Division Database"; }
    }
}
