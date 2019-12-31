using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;


namespace CCRevitTools
{
    #region Print Sets
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class PrintSets : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document currentDoc = uiApp.ActiveUIDocument.Document;

            //get the PrintManager from the current document
            PrintManager printManager = currentDoc.PrintManager;
            if (printManager.PrintRange != PrintRange.Select)
            {
                printManager.PrintRange = PrintRange.Select;
            }
            //get the ViewSheetSetting which manages the view / sheet set info of current document
            ViewSheetSetting viewSheetSetting = printManager.ViewSheetSetting;

            List<ElementId> revIDs = Revision.GetAllRevisionIds(currentDoc).ToList();
            List<ElementId> activeRevs = new List<ElementId>();

            StringBuilder sb = new StringBuilder();

            foreach (ElementId eleid in revIDs)
            {
                Revision rev = currentDoc.GetElement(eleid) as Revision;
                if (rev.Issued == false)
                {
                    activeRevs.Add(eleid);
                }
            }

            FilteredElementCollector sheetsetcol = new FilteredElementCollector(currentDoc);
            List<Element> viewsetlist = sheetsetcol.OfClass(typeof(ViewSheetSet)).ToList();

            foreach (Element elset in viewsetlist)
            {
                ViewSheetSet vss = elset as ViewSheetSet;
                string name = vss.Name;
                foreach (ElementId id in activeRevs)
                {
                    Revision rev = currentDoc.GetElement(id) as Revision;
                    string revname = rev.Name;
                    if (name == revname)
                    {
                        viewSheetSetting.CurrentViewSheetSet = vss;
                        viewSheetSetting.Delete();
                    }
                }
            }

            foreach (ElementId revid in activeRevs)
            {
                Revision rev = currentDoc.GetElement(revid) as Revision;
                string revname = rev.Name;

                FilteredElementCollector sheetCollector = new FilteredElementCollector(currentDoc);
                sheetCollector.OfCategory(BuiltInCategory.OST_Sheets);

                //create a new  ViewSet - add views to it that match the desired criteria
                ViewSet sheetset = new ViewSet();

                foreach (ViewSheet vs in sheetCollector)
                {
                    List<ElementId> revset = vs.GetAllRevisionIds().ToList();
                    if (revset.Contains(revid))
                    {
                        sheetset.Insert(vs);
                    }
                }

                viewSheetSetting.CurrentViewSheetSet.Views = sheetset;
                viewSheetSetting.SaveAs(revname);

                sb.AppendLine(revname + " Contains " + sheetset.Size + " sheets.");
            }
            string outlist = sb.ToString();
            TaskDialog.Show("View Set", outlist);
            return Result.Succeeded;
        }
    }
    #endregion
}