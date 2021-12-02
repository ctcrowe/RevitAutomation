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
    internal static class UpdateTab
    {
        private static string dllpath = Assembly.GetExecutingAssembly().Location;
        public const string PName = "AI Update";
        public const string TBName = "Parameter Value";

        public static void CreatePanel(UIControlledApplication uiApp)
        {
            RibbonPanel Panel = uiApp.CreateRibbonPanel(CCRibbon.tabName, PName);
            
            TextBoxData tbd = new TextBoxData(TBName);
            TextBox tb = Panel.AddItem(tbd) as TextBox;
            tb.Width = 350;
            
            PushButtonData MFBData = new PushButtonData (
                "Set Masterformat",
                "Set Masterformat",
                @dllpath,
                "CC_Plugin.SetMasterformat");
            MFBData.ToolTip = "Set Masterformat Value for an Element based on Text Box Entry.";
            PushButton MFButton = Panel.AddItem(MFBData) as PushButton;
        }
        public static string GetText(this UIApplication app)
        {
            string text = "";
            try
            {
                var panels = app.GetRibbonPanels(CCRibbon.tabName);
                var panel = panels.Where(x => x.Name == PName).First();
                var items = panel.GetItems();
                var item = items.Where(x => x.Name == TBName).First() as TextBox;
                text = item.Value.ToString();
            }
            catch (Exception e) { e.OutputError(); }
            return text;
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
            var text = commandData.Application.GetText();
            int numb;
            if(int.TryParse(text, out numb))
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                Selection sel = commandData.Application.ActiveUIDocument.Selection;
                ISelectionFilter selectionFilter = new EleSelectionFilter();

                Reference ChangedObject = sel.PickObject(ObjectType.Element, selectionFilter);
                FamilyInstance inst = doc.GetElement(ChangedObject.ElementId) as FamilyInstance;
                FamilySymbol symb = inst.Symbol;
                NeuralNetwork net = MasterformatNetwork.GetNetwork(CMDLibrary.WriteNull);

                Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                s.TextInput = symb.Family.Name;
                s.DesiredOutput = new double[net.Layers.Last().Biases.Count()];
                s.DesiredOutput[numb] = 1;
                MasterformatNetwork.Propogate(s, CMDLibrary.WriteNull);

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
