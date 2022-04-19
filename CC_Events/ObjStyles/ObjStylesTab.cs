using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public class ObjStylesTab
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Plugin.dll";
            return dll;
        }
        public static void ObjTab(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Object Styles");

            PushButtonData B1 = new PushButtonData(
                "Get Obj Styles",
                "Get Obj Styles",
                dllpath(),
                "CC_Plugin.GetObjStyles");

            PushButtonData B2 = new PushButtonData(
                "Set Obj Styles",
                "Set Obj Styles",
                dllpath(),
                "CC_Plugin.SetObjStyles");

            PushButtonData B3 = new PushButtonData(
                "Param Check",
                "Param Check",
                dllpath(),
                "CC_Plugin.ParamCheck");

            List<RibbonItem> DBButtons5 = new List<RibbonItem>();
            DBButtons5.AddRange(CPanel.AddStackedItems(B1, B2, B3));
        }
    }
}
