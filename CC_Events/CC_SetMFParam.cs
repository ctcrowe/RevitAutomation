using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

using System;
using System.IO;
using System.Reflection;

using CC_Library.Parameters;

namespace CC_Plugin
{
    public static class MFPanel
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
                    doc.SetFamilyParam(CCParameter.Masterformat, tb.Value.ToString());
                    t.Commit();
                }
            }
        }
        public static void SetMasterformat(this Document doc, string Value)
        {
            if (doc.IsFamilyDocument)
            {
                if (Value == null)
                {
                    if (!doc.CheckStringParam(CCParameter.Masterformat))
                    {
                        Random rand = new Random();
                        int numb = rand.Next(25);
                        doc.SetFamilyParam(CCParameter.Masterformat, numb.ToString());
                    }
                }
                else
                    doc.SetFamilyParam(CCParameter.Masterformat, Value);
            }
        }
    }
}
