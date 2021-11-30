using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CC_Library;
using CC_Library.Parameters;
using CC_Library.Predictions;
using CC_Plugin.Parameters;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using System.Reflection;

namespace CC_Plugin
{
    internal class UpdateTab
    {
        private static string dllpath = Assembly.GetExecutingAssembly().Location;

        public static void CreatePanel(UIControlledApplication uiApp)
        {
            RibbonPanel Panel = uiApp.CreateRibbonPanel(CCRibbon.tabName, "Text Modify");
            
            TextBoxData tbd = new TextBoxData("Enter");
            TextBox tb = Panel.AddItem(tbd) as TextBox;
            
            PushButtonData MFBData = new PushButtonData
                "Set Masterformat",
                "Set Masterformat",
                @dllpath,
                "CC_Plugin.SetMasterformat");
            MFBData.ToolTip = "Set Masterformat Value for an Element based on Text Box Entry.";
            PushButton MFButton = Panel.AddItem(MFBData) as PushButton;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class SetMasterformat : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication app = commandData.Application;
            var panels = app.GetRibbonPanels(CCRibbon.tabName)
            var panel = panels.Where(x => x.Name == "Text Modify").First();
            var item = panel.GetItems().Where(x => x.Name == "Enter").First() as TextBox;
            var text = item.Value;
            
            int numb;
            if(int.TryParse(item.Value.ToString(), out numb))
            {
                Selection sel = args.Application.ActiveUIDocument.Selection;
                ISelectionFilter selectionFilter = new EleSelectionFilter();

                Reference ChangedObject = sel.PickObject(ObjectType.Element, selectionFilter);
                FamilyInstance inst = doc.GetElement(ChangedObject.ElementId) as FamilyInstance;
                FamilySymbol symb = inst.Symbol;
                MasterformatNetwork net = new MasterformatNetwork();

                Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                s.TextInput = symb.Family.Name;
                s.DesiredOutput = new double[net.Network.Layers.Last().Biases.Count()];
                s.DesiredOutput[numb] = 1;
                net.Propogate(s, CMDLibrary.WriteNull);

                using (Transaction t = new Transaction(doc, "Set Param"))
                {
                    t.Start();
                    symb.SetElementParam(Params.Masterformat, numb.ToString());
                    t.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
    public class EleSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            FamilyInstance inst = element as FamilyInstance;
            if (inst != null)
                return true;
            else
                return false;
        }
        public bool AllowReference(Reference refer, XYZ point) { return true; }
    }
}
