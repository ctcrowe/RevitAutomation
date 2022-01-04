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
            tb.EnterPressed += EnterPressed;

            ComboBoxData cbd = new ComboBoxData("Update Type");
            ComboBox box = Panel.AddItem(cbd) as ComboBox;
            box.AddItem(new ComboBoxMemberData("Masterformat", "Masterformat"));
            box.AddItem(new ComboBoxMemberData("Occupant Load Factor", "Occupant Load Factor"));
        }
        private static void EnterPressed(object sender, TextBoxEnterPressedEventArgs args)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Value as string;
            var combotype = args.Application.GetComboData();
            if(combotype == "Predictive")
                combotype = "Masterformat";
            switch(combotype)
            {
                default:
                case "Masterformat":
                    args.Application.SetMasterformat(text);
                    break;
                case "Occupant Load Factor":
                    break;
            }
        }
        private static string GetComboData(this UIApplication app)
        {
            string val = "";
            try
            {
                var panels = app.GetRibbonPanels(CCRibbon.tabName);
                var panel = panels.Where(x => x.Name == PName).First();
                var items = panel.GetItems();
                var item = items.Where(x => x.ItemType == RibbonItemType.ComboBox).First();
                var cb = item as ComboBox;
                val = cb.Current.Name;
            }
            catch (Exception e) { e.OutputError(); }
            return val;
        }
    }
    public static class CMD_SetMasterformat
    {
        public static void SetMasterformat( this UIApplication app, string text )
            //ExternalCommandData commandData,
            //ref string message,
            //ElementSet elements)
        {
            int numb;
            if(int.TryParse(text, out numb))
            {
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
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
        }
    }
    public static class CMD_SetOLF
    {
        public static void SetOLF( this UIApplication app, string text )
            //ExternalCommandData commandData,
            //ref string message,
            //ElementSet elements)
        {
            int numb;
            if(int.TryParse(text, out numb))
            {
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
                ISelectionFilter selectionFilter = new RoomSelectionFilter();

                Reference ChangedObject = sel.PickObject(ObjectType.Element, selectionFilter);
                Room r = doc.GetElement(ChangedObject.ElementId) as Room;
                NeuralNetwork net = OLFNetwork.GetNetwork(CMDLibrary.WriteNull);

                Sample s = new Sample(CC_Library.Datatypes.Datatype.OccupantLoadFactor);
                s.TextInput = r.Name;
                s.DesiredOutput = new double[net.Layers.Last().Biases.Count()];
                s.DesiredOutput[numb] = 1;
                OLFNetwork.Propogate(s, CMDLibrary.WriteNull);

                using (Transaction t = new Transaction(doc, "Set Param"))
                {
                    t.Start();
                    symb.SetElementParam(Params.Masterformat, numb.ToString());
                    t.Commit();
                }
            }
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
    public class RoomSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            Room r = element as Room;
            if (r != null)
                return true;
            else
                return false;
        }
        public bool AllowReference(Reference refer, XYZ point) { return true; }
    }
}
