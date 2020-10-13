using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public class QCTab
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Plugin.dll";
            return dll;
        }
        public static void QCPanel(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Quality Control");

            PushButtonData b1Data = new PushButtonData(
                "Project Cleanup",
                "Project\r\nCleanup",
                @dllpath(),
                "CC_Plugin.ProjectCleanup");
            b1Data.ToolTip = "Remove unused View Templates, unused View Filters, and unused Elevation Markers";

            PushButton PB1 = CPanel.AddItem(b1Data) as PushButton;

            PushButtonData B2Data = new PushButtonData(
                "Update Line Styles",
                "Update Line Styles",
                @dllpath(),
                "CC_Plugin.ChangeLineStyle");

            PushButtonData B3Data = new PushButtonData(
                "List Line Styles",
                "List Line Styles",
                dllpath(),
                "CC_Plugin.ListLineStyles");

            PushButtonData B4Data = new PushButtonData(
                "Select Lines",
                "Select Lines",
                dllpath(),
                "CC_Plugin.SelectLines");

            List<RibbonItem> DBButtons2 = new List<RibbonItem>();
            DBButtons2.AddRange(CPanel.AddStackedItems(B2Data, B3Data, B4Data));

            PushButtonData B5Data = new PushButtonData(
                "Update Text Styles",
                "Update Text Styles",
                @dllpath(),
                "CC_Plugin.ChangeTextStyle");

            PushButtonData B6Data = new PushButtonData(
                "List Text Styles",
                "List Text Styles",
                dllpath(),
                "CC_Plugin.ListTextStyles");

            PushButtonData B7Data = new PushButtonData(
                "Select Notes",
                "Select Notes",
                dllpath(),
                "CC_Plugin.SelectNotes");

            List<RibbonItem> DBButtons3 = new List<RibbonItem>();
            DBButtons2.AddRange(CPanel.AddStackedItems(B5Data, B6Data, B7Data));

            PushButtonData B8Data = new PushButtonData(
                "Capture Families",
                "Capture Families",
                dllpath(),
                "CC_Plugin.CollectFamilies");

            PushButtonData B9Data = new PushButtonData(
                "Capture Rooms",
                "Capture Rooms",
                dllpath(),
                "CC_Plugin.CollectRooms");

            PushButtonData B11Data = new PushButtonData(
                "Capture Walls",
                "Capture Walls",
                dllpath(),
                "CC_Plugin.CollectWalls");


            List<RibbonItem> DBButtons4 = new List<RibbonItem>();
            DBButtons4.AddRange(CPanel.AddStackedItems(B8Data, B9Data, B11Data));

            PushButtonData B10Data = new PushButtonData(
                "Generate Finish Floors",
                "Generate Finish Floors",
                dllpath(),
                "CC_Plugin.GenerateFinishFloors");
            PushButton FinFloors = CPanel.AddItem(B10Data) as PushButton;

            PushButtonData B12Data = new PushButtonData(
                "Collect Boundaries",
                "Collect Boundaries",
                dllpath(),
                "CC_Plugin.CollectRoomBounds");

            PushButtonData B13 = new PushButtonData(
                "Show Category",
                "Show Category",
                dllpath(),
                "CC_Plugin.ShowCategory");


            List<RibbonItem> DBButtons5 = new List<RibbonItem>();
            DBButtons5.AddRange(CPanel.AddStackedItems(B12Data, B13));
        }
    }
}
