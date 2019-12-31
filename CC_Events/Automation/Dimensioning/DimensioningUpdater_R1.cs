using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace CC_Plugin
{
    public class Dimensioner
    {
        public static void OnStartup(UIControlledApplication application)
        {
            DimensionUpdater updater = new DimensionUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater, true);

            ElementCategoryFilter dimFilter = new ElementCategoryFilter(BuiltInCategory.OST_Dimensions);
            
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), refFilter, Element.GetChangeTypeElementAddition());
        }

        public static void OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            DimensionUpdater updater = new DimensionUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
        }
    }

    public class DimensionUpdater : IUpdater
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";
        private static List<Param> Parameters = new List<Param> {
            ParameterLibrary.OverallDepth, ParameterLibrary.OverallHeight, ParameterLibrary.OverallWidth,
            ParameterLibrary.ODepthControl, ParameterLibrary.OWidthControl, ParameterLibrary.OHeightControl};

        static AddInId appId;
        static UpdaterId updaterId;

        public DimensionUpdater(AddInId id)
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("438ae6fe-536b-4e8d-bd32-26afb7cdebe8"));
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();

            if (File.Exists(SharedParams))
            {
                File.Delete(SharedParams);
            }
            using (FileStream stream = File.Create(SharedParams))
            {
                stream.Close();
            }

            Application App = doc.Application;
            App.SharedParametersFilename = SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();

            foreach (Param p in Parameters)
            {
                if (deffile.Groups.get_Item(p.ParamGroup) == null)
                {
                    deffile.Groups.Create(p.ParamGroup);
                }
                if (deffile.Groups.get_Item(p.ParamGroup).Definitions.get_Item(p.name) == null)
                {
                    ExternalDefinitionCreationOptions opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.Invalid);
                    switch (p.type)
                    {
                        case ParamType.Int:
                            opt.Type = ParameterType.Integer;
                            break;
                        case ParamType.String:
                            opt.Type = ParameterType.Text;
                            break;
                        case ParamType.Double:
                            opt.Type = ParameterType.Number;
                            break;
                        case ParamType.Bool:
                            opt.Type = ParameterType.YesNo;
                            break;
                        case ParamType.Length:
                            opt.Type = ParameterType.Length;
                            break;
                        case ParamType.Material:
                            opt.Type = ParameterType.Material;
                            break;
                        case ParamType.Area:
                            opt.Type = ParameterType.Area;
                            break;
                    }
                    if (!p.Vis)
                    {
                        opt.Visible = false;
                    }
                    if (p.Description != null)
                    {
                        opt.Description = p.Description;
                    }
                    if (!p.UsrMod)
                    {
                        opt.UserModifiable = false;
                    }
                    opt.GUID = p.ID;
                    Definition def = deffile.Groups.get_Item(p.ParamGroup).Definitions.Create(opt);
                }

                if (doc.IsFamilyDocument)
                {
                    try
                    {
                        Category cat = doc.OwnerFamily.FamilyCategory;
                        DefinitionGroup group = deffile.Groups.get_Item(p.ParamGroup);
                        ExternalDefinition def = group.Definitions.get_Item(p.name) as ExternalDefinition;
                        doc.FamilyManager.AddParameter(def, p.BuiltInGroup, p.Inst);
                    }
                    catch
                    { }
                }
                else
                {
                    try
                    {
                        CategorySet set = new CategorySet();
                        foreach (BuiltInCategory cat in p.Categories)
                        {
                            if (!set.Contains(Category.GetCategory(doc, cat)))
                                set.Insert(Category.GetCategory(doc, cat));
                        }
                        if (set.Size > 0)
                        {
                            DefinitionGroup group = deffile.Groups.get_Item(p.ParamGroup);
                            Definition def = group.Definitions.get_Item(p.name);
                            if (p.Inst)
                            {
                                InstanceBinding binding = new InstanceBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                            else
                            {
                                TypeBinding binding = new TypeBinding(set);
                                doc.ParameterBindings.Insert(def, binding);
                            }
                        }
                    }
                    catch
                    { }
                }
            }
        }
        public string GetAdditionalInformation() { return "Adds Typical Dimensioning Parameters to the Project"; }
        public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
        public UpdaterId GetUpdaterId() { return updaterId; }
        public string GetUpdaterName() { return "Dimension Updater"; }
    }
}