using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CC_Library.Parameters;
using CC_Library.Datatypes;
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
                    try
                    {
                        Element symb = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).FirstOrDefault();
                        if (symb != null)
                        {
                            Parameter p = symb.get_Parameter(Params.Masterformat.Guid);
                            UpdateMFDB updater = new UpdateMFDB(id);
                            UpdaterRegistry.RegisterUpdater(updater, doc, true);
                            ElementClassFilter symbs = new ElementClassFilter(typeof(FamilySymbol));
                            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), symbs, Element.GetChangeTypeParameter(p.Id));
                        }
                    }
                    catch (Exception e) { e.OutputError(); }
                }
                tg.Commit();
            }
        }
        public static void ProjectShutdown(AddInId id, Document doc)
        {
            UpdateMFDB updater = new UpdateMFDB(id);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId(), doc);
        }
        public void Execute(UpdaterData data)
        {
            try
            {
                Document doc = data.GetDocument();
                var Eles = data.GetModifiedElementIds().ToList();
                foreach (var eid in Eles)
                {
                    try
                    {
                        Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                        MasterformatNetwork net = new MasterformatNetwork(new WriteToCMDLine(CMDLibrary.WriteNull));
                        var ele = doc.GetElement(eid) as FamilySymbol;
                        try { s.TextInput = ele.FamilyName + " " + ele.Name; }
                        catch (Exception e) { e.OutputError(); }
                        s.DesiredOutput = new double[net.Network.Layers.Last().Biases.Count()];
                        s.DesiredOutput[int.Parse(ele.GetElementParam(Params.Masterformat))] = 1;
                        net.Propogate(s, new WriteToCMDLine(CMDLibrary.WriteMull));
                    }
                    catch (Exception e)
                    {
                        e.OutputError();
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
