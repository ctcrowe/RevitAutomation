using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

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
        }
    }
}
