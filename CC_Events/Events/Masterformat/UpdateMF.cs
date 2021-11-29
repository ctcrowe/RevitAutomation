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
            tb.EnterPressed += CallTextCommand;
            
            PushButtonData MFBData = new PushButtonData
                (
                );
            PushButton MFButton = Panel.AddItem(MFBData) as PushButton;
        }
        public static void CallTextCommand(object sender, TextBoxEnterPressedEventArgs args)
        {
            Autodesk.Revit.UI.TextBox tb = sender as Autodesk.Revit.UI.TextBox;
            Document doc = args.Application.ActiveUIDocument.Document;

            int numb;
            if(int.TryParse(tb.Value.ToString(), out numb))
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
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            CCPaintPanel.PaintByMaterial(uiDoc, Params.Finish);
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
