using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

using System.IO;
using System.Reflection;

namespace CC_Plugin
{
    public class MFPanel
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Events.dll";
            return dll;
        }
        public static void MFTab(UIControlledApplication uiApp, string tabname)
        {

            RibbonPanel Panel = uiApp.CreateRibbonPanel(tabname, "Masteformat");

            TextBoxData data = new TextBoxData("Masterformat Number");
            TextBox tb = Panel.AddItem(data) as TextBox;
            tb.EnterPressed += set;
        }
        public static void set(object sender, TextBoxEnterPressedEventArgs args)
        {
            Document doc = args.Application.ActiveUIDocument.Document;
            if (doc.IsFamilyDocument)
            {
                TextBox tb = sender as TextBox;
                using (Transaction t = new Transaction(doc, "Set MF Param"))
                {
                    t.Start();
                    MFParam.Set(args.Application.ActiveUIDocument.Document, tb.Value.ToString());
                    t.Commit();
                }
            }
        }
    }
}
