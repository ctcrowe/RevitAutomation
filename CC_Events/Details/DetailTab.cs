using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.Revit.UI;

namespace CC_Plugin.Details
{
    public class DetailPanel
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Plugin.dll";
            return dll;
        }
        public static void DetailTab(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Details");

            PushButtonData b1Data = new PushButtonData(
                "Export Detail",
                "Export Detail",
                @dllpath(),
                "CC_Plugin.Details.DetailImageButton");

            PushButton PB1 = CPanel.AddItem(b1Data) as PushButton;
        }
    }
}
