using CC_CoreData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;

namespace CC_Events
{
    public class ProjectDataIO
    {
        public static PData PDataFromRevit(Document currentDoc)
        {
            PData data = new PData();
            data.Date = DateTime.Now;

            //list all views
            FilteredElementCollector viewCollector = new FilteredElementCollector(currentDoc);
            viewCollector.OfCategory(BuiltInCategory.OST_Views);

            //list all sheets
            FilteredElementCollector sheetCollector = new FilteredElementCollector(currentDoc);
            sheetCollector.OfCategory(BuiltInCategory.OST_Sheets);

            //list all TextNoteTypes
            FilteredElementCollector textCollector = new FilteredElementCollector(currentDoc);
            ICollection<ElementId> textstyles = textCollector.OfClass(typeof(TextNoteType)).ToElementIds().ToList();

            //list all Materials
            FilteredElementCollector matCollector = new FilteredElementCollector(currentDoc);
            ICollection<ElementId> materials = matCollector.OfClass(typeof(Material)).ToElementIds().ToList();

            //list all Groups
            FilteredElementCollector GroupCollector = new FilteredElementCollector(currentDoc);
            ICollection<ElementId> groups = GroupCollector.OfClass(typeof(GroupType)).ToElementIds().ToList();

            //list all families
            FilteredElementCollector FamCollector = new FilteredElementCollector(currentDoc);
            ICollection<ElementId> Fams = FamCollector.OfClass(typeof(Family)).ToElementIds().ToList();

            //list all elements
            FilteredElementCollector eleCollector = new FilteredElementCollector(currentDoc);
            ICollection<Element> eles = eleCollector.OfClass(typeof(FamilyInstance)).ToList();

            FilteredElementCollector noteCollector = new FilteredElementCollector(currentDoc);
            ICollection<ElementId> textnotes = noteCollector.OfClass(typeof(TextNote)).ToElementIds().ToList();

            //crossreference lists and find unused views
            List<View> unusedviews = new List<View>();
            List<ViewSheet> projectsheets = new List<ViewSheet>();
            List<GroupType> pGroups = new List<GroupType>();
            List<View> totalviews = new List<View>();

            //check if views are in use
            foreach (View currentView in viewCollector)
            {
                totalviews.Add(currentView);
                //check if current view is a legend
                if (currentView.ViewType != ViewType.Legend
                    && currentView.IsTemplate == false
                    && Viewport.CanAddViewToSheet(currentDoc, sheetCollector.FirstElement().Id, currentView.Id) != false)
                {
                    //add view to list of views to delete
                    unusedviews.Add(currentView);
                }
            }

            //determine if groups are purgeable
            foreach (GroupType curGroup in GroupCollector)
            {
                //check if group can be deleted)
                if (curGroup.Groups.Size == 0)
                {
                    pGroups.Add(curGroup);
                }

            }

            //create list of sheets
            foreach (ViewSheet currentSheet in sheetCollector)
            {
                projectsheets.Add(currentSheet);
            }

            data.ticks = data.Date.Ticks;
            data.TotalViews = viewCollector.Count();
            data.UnusedViews = unusedviews.Count();
            data.TotalSheets = projectsheets.Count();
            data.TextStyles = textstyles.Count();
            data.Materials = materials.Count();
            data.Groups = groups.Count();
            data.PurgeableGroups = pGroups.Count();
            data.Families = Fams.Count();
            data.FamInstances = eles.Count();
            data.TextNotes = textnotes.Count();
            return data;
        }
        public static void PDataParamUpdates(Document currentDoc, PData data)
        {
            if (Directory.Exists(DBIdentifiers.Database))
            {
                Application App = currentDoc.Application;
                App.SharedParametersFilename = DBIdentifiers.SharedParams;
                DefinitionFile deffile = App.OpenSharedParameterFile();
                ProjectInfo info = currentDoc.ProjectInformation;
                Parameter TV = info.get_Parameter(ParameterLibrary.TotalViews.ID);
                TV.Set(Convert.ToInt32(data.TotalViews));
                Parameter UV = info.get_Parameter(ParameterLibrary.UnusedViews.ID);
                UV.Set(Convert.ToInt32(data.UnusedViews));
                Parameter TS = info.get_Parameter(ParameterLibrary.TotalSheets.ID);
                TS.Set(Convert.ToInt32(data.TotalSheets));
                Parameter TXT = info.get_Parameter(ParameterLibrary.TextStyles.ID);
                TXT.Set(Convert.ToInt32(data.TextStyles));
                Parameter MAT = info.get_Parameter(ParameterLibrary.Materials.ID);
                MAT.Set(Convert.ToInt32(data.Materials));
                Parameter GRP = info.get_Parameter(ParameterLibrary.Groups.ID);
                GRP.Set(Convert.ToInt32(data.Groups));
                Parameter PGP = info.get_Parameter(ParameterLibrary.PurgeableGroups.ID);
                PGP.Set(Convert.ToInt32(data.PurgeableGroups));
                Parameter FAM = info.get_Parameter(ParameterLibrary.Families.ID);
                FAM.Set(Convert.ToInt32(data.Families));
                Parameter ELE = info.get_Parameter(ParameterLibrary.FamilyInstances.ID);
                ELE.Set(Convert.ToInt32(data.FamInstances));
                Parameter TNT = info.get_Parameter(ParameterLibrary.TextNotes.ID);
                TNT.Set(Convert.ToInt32(data.TextNotes));
            }
        }
        public static void UpdateSplashPage(Document currentDoc)
        {
            string fp = currentDoc.PathName;
            BasicFileInfo bfi = BasicFileInfo.Extract(fp);
            string proj = bfi.CentralPath.Split('\\').Last().Split('.').First();
            string pdir = DBIdentifiers.Database + "ProjectData\\" + proj + "\\";
            if (Directory.Exists(pdir))
            {
                string[] files = Directory.GetFiles(pdir);
                PData data = PDataFromRevit(currentDoc);
                PDataParamUpdates(currentDoc, data);
            }
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class SynchronizeSplashData : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document currentDoc = commandData.Application.ActiveUIDocument.Document;
            Application App = currentDoc.Application;
            App.SharedParametersFilename = DBIdentifiers.SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();

            using (Transaction trans = new Transaction(currentDoc, "Synch Data"))
            {
                trans.Start();
                ParamConstruction.ParameterGroupSetup(deffile, ParameterGroup.ProjectAnalysisParams);
                ParamConstruction.ParameterGroupBindingSetup(deffile, currentDoc, ParameterGroup.ProjectAnalysisParams);
                ProjectDataIO.UpdateSplashPage(currentDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
}