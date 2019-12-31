using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    class DocumentSaved
    {
        public static void Event(object sender, DocumentSavedEventArgs args)
        {
            SaveFamily.Main(args.Document);
        }
    }
}