using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public class RefPlaneMaker
    {
        /*
        public static void OnStartup(UIControlledApplication application)
        {
            RefPlaneUpdater rpupdater = new RefPlaneUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(rpupdater, true);
            
            ElementClassFilter refFilter = new ElementClassFilter(typeof(ReferencePlane));

            UpdaterRegistry.AddTrigger(rpupdater.GetUpdaterId(), refFilter, Element.GetChangeTypeElementAddition());
        }

        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            RefPlaneUpdater updater = new RefPlaneUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
        */
    }

    //public class RefPlaneUpdater : IUpdater
    //{
        /*
        private static List<CCcategory> CCcategories = new List<CCcategory>
        {
            new CCcategory("CL", 1, new byte[3]{252, 198, 3 }),
            new CCcategory("EXIST", 1, new byte[3]{252, 69, 3}),
            new CCcategory("EXTREMES", 1, new byte[3]{169, 3, 252})
        };

        static AddInId appId;
        static UpdaterId updaterId;

        public RefPlaneUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("1a82126d-83b5-4579-bf3e-d6bf55286345"));
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            
            Category rp = Category.GetCategory(doc, BuiltInCategory.OST_CLines);

            foreach (CCcategory cat in CCcategories)
            {
                Category subcat;

                if (!rp.SubCategories.Contains(cat.name))
                {
                    subcat = doc.Settings.Categories.NewSubcategory(rp, cat.name);
                }

                else
                {
                    subcat = rp.SubCategories.get_Item(cat.name);
                }

                subcat.SetLineWeight(cat.lineweight, GraphicsStyleType.Projection);
                subcat.LineColor = new Color(cat.color[0], cat.color[1], cat.color[2]);
            }
        }
        public string GetAdditionalInformation() { return "Adds typical reference plane subcategories to a project."; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Charlie Crowe Reference Plane Categories Updater"; }
        */
    //}
}