using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin.TypeNaming
{
    public class TypeNamingUpdater : IUpdater
    {
        public void Execute(UpdaterData data)
        {
            List<ElementId> eids = data.GetModifiedElementIds().ToList();
            Document doc = data.GetDocument();
            if(!doc.IsFamilyDocument)
            {
                foreach (ElementId eid in eids)
                {
                    Element ele = doc.GetElement(eid);
                    Category cat = null;
                    var FS = ele as FamilySymbol;
                    WallType wt = ele as WallType;
                    try { cat = FS.Family.FamilyCategory; } catch { }
                    try { cat = wt.Category; } catch { }
                    if (cat != null)
                    {
                        switch (cat.Id.IntegerValue)
                        {
                            default:
                                break;
                            case (int)BuiltInCategory.OST_Windows:
                                try { ele.SetWindowTypeName(); }
                                catch { }
                                break;
                            case (int)BuiltInCategory.OST_Casework:
                                try { }
                                catch (Exception e) {e.OutputError(); }
                                break;
                                /*
                            case (int)BuiltInCategory.OST_Doors:
                                try { ele.SetDoorTypeName(); }
                                catch { TaskDialog.Show("Error", "Failed to update Door Type Name"); }
                                break;
                            case (int)BuiltInCategory.OST_Walls:
                                try { ele.SetWallTypeName(); }
                                catch { TaskDialog.Show("Error", "Faild to update Wall Type Name"); }
                                break;
                                */
                        }
                    }
                }
            }
        }
        public static void OnStartup(UIControlledApplication application)
        {
            TypeNamingUpdater updater = new TypeNamingUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);
            ElementId WindowHeight = new ElementId(BuiltInParameter.WINDOW_HEIGHT);
            ElementId WindowWidth = new ElementId(BuiltInParameter.WINDOW_WIDTH);
            ElementId TypeMark = new ElementId(BuiltInParameter.ALL_MODEL_TYPE_MARK);
            ElementClassFilter symbols = new ElementClassFilter(typeof(FamilySymbol));
            //ElementClassFilter walls = new ElementClassFilter(typeof(WallType));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), symbols, Element.GetChangeTypeParameter(WindowHeight));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), symbols, Element.GetChangeTypeParameter(WindowWidth));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), symbols, Element.GetChangeTypeParameter(TypeMark));
            //UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), walls, Element.GetChangeTypeParameter());
        }
        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            TypeNamingUpdater updater = new TypeNamingUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        public TypeNamingUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("702ee21d-110d-40db-b065-d9b08d9a7cab"));
        }
        static AddInId appId;
        static UpdaterId updaterId;
        public string GetAdditionalInformation() { return ""; }
        public ChangePriority GetChangePriority() { return ChangePriority.DoorsOpeningsWindows; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Change Types"; }
    }
}
