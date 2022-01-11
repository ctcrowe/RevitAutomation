using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace CC_Plugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class SetOLF : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            using (Transaction t = new Transaction(doc, "Set OLF"))
            {
                t.Start();
                doc.UpdateLoadFactor();
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}