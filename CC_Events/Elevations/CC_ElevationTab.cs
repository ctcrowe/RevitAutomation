using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.Revit.UI;

namespace CC_Events
{
    public class ElevationTab
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Events.dll";
            return dll;
        }
        public static void ElevationPanel(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel panel = uiApp.CreateRibbonPanel(tabname, "Elevations");

            PushButtonData b1data = new PushButtonData(
                "Generate Floor Views",
                "Generate Floor Views",
                @dllpath(),
                "CC_Events.CreateFloorViews");

            PushButtonData b2data = new PushButtonData(
                "Room Views",
                "Room Views",
                @dllpath(),
                "CC_Events.CreateRoomViews");

            PushButtonData b3data = new PushButtonData(
                "View Testing",
                "View Testing",
                @dllpath(),
                "CC_Events.CreateViews");

            List<RibbonItem> DBButtons = new List<RibbonItem>();
            DBButtons.AddRange(panel.AddStackedItems(b1data, b2data, b3data));
        }
    }
}
