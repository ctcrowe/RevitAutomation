using CC_CoreData;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace CC_Events
{
    public class DatabasePanel
    {
        internal static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Events.dll";
            return dll;
        }
        public static void DBPanel(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel Panel = uiApp.CreateRibbonPanel(tabname, "Database");

            PushButtonData Analysis = new PushButtonData(
                "Run Fixture Calcs",
                "Run Fixture Calcs",
                dllpath(),
                "CC_Events.RunCodeAnalysis");
            Analysis.ToolTip = "Run plumbing fixture calcs for all rooms in the project.";
            PushButton AnalysisButton = Panel.AddItem(Analysis) as PushButton;

            Panel.AddSeparator();

            PushButtonData Notes = new PushButtonData(
                "Notes",
                "Notes",
                @dllpath(),
                "CC_Events.RevitNotes");
            PushButton NoteButton = Panel.AddItem(Notes) as PushButton;

            Panel.AddSeparator();

            PushButtonData AddParameters = new PushButtonData(
                "Add Parameters",
                "Add Parameters",
                @dllpath(),
                "CC_Events.AddParameters");

            PushButtonData ResetParameters = new PushButtonData(
                "Reset Paraeters",
                "Reset Parameters",
                @dllpath(),
                "CC_Events.ResetParams");

            ComboBoxData DatabaseOptionData = new ComboBoxData("Database Options");
            IList<RibbonItem> Basics = Panel.AddStackedItems(AddParameters, ResetParameters, DatabaseOptionData);

            if (Basics.Count > 1)
            {
                ComboBox dbBox = Basics[2] as ComboBox;
                if (dbBox != null)
                {
                    ComboBoxMemberData MaterialOption = new ComboBoxMemberData("Materials", "Materials");
                    MaterialOption.GroupName = "Database";
                    dbBox.AddItem(MaterialOption);

                    ComboBoxMemberData EquipmentOption = new ComboBoxMemberData("Equipment", "Equipment");
                    EquipmentOption.GroupName = "Database";
                    dbBox.AddItem(EquipmentOption);

                    ComboBoxMemberData StaffOption = new ComboBoxMemberData("Facilities", "Facilities");
                    StaffOption.GroupName = "Database";
                    dbBox.AddItem(StaffOption);

                    ComboBoxMemberData DoorsOption = new ComboBoxMemberData("Doors", "Doors");
                    DoorsOption.GroupName = "Database";
                    dbBox.AddItem(DoorsOption);
                    
                    ComboBoxMemberData ProjectOption = new ComboBoxMemberData("Project", "Project");
                    ProjectOption.GroupName = "Database";
                    dbBox.AddItem(ProjectOption);

                    dbBox.CurrentChanged += new EventHandler<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs>(RegenComboBox);
                }

                PushButtonData B1Data = new PushButtonData(
                    "Import Data",
                    "Import Data",
                    @dllpath(),
                    "CC_Events.ReadDatabase");

                PushButtonData B2Data = new PushButtonData(
                    "Export Data",
                    "Export Data",
                    @dllpath(),
                    "CC_Events.WriteDatabase");

                List<RibbonItem> MatDBButtons = new List<RibbonItem>();
                MatDBButtons.AddRange(Panel.AddStackedItems(B1Data, B2Data));
            }
        }
        public static void RegenComboBox(object sender, Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs e)
        {
            ComboBoxMember combodata = e.NewValue;
            RevitDatabase.ChangeComboBoxValue(combodata.Name);
        }
    }
    #region buttons
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class RunCodeAnalysis : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document currentDoc = commandData.Application.ActiveUIDocument.Document;

            using (Transaction trans = new Transaction(currentDoc, "Synch Data"))
            {
                trans.Start();
                RevitCodeAnalysis.CodeAnalysis(currentDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class AddParameters : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document currentDoc = commandData.Application.ActiveUIDocument.Document;

            using (Transaction trans = new Transaction(currentDoc, "Add Params"))
            {
                trans.Start();
                int c = RevitParamSetup.AddProjectParameters(currentDoc);
                TaskDialog.Show("Success!", c + " parameters have been added to the current file!");
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class ResetParams : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Application App = commandData.Application.Application;
            int c = ParamFileSetup.ParameterReset(App);
            TaskDialog.Show("Success!", "Your shared parameter file has been reset with " + c + " standard parameters included");
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class ReadDatabase : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            /*
            Document currentDoc = commandData.Application.ActiveUIDocument.Document;
            string combosetting = RevitDatabase.ReadComboBoxValue();
            switch (combosetting)
            {
                case "Project":
                    ProjectDataIO.UpdatePData(currentDoc);
                    break;
                case "Equipment":
                    RevitEquipment.ReadEquipment(currentDoc);
                    break;
                case "Facilities":
                    RevitFacilities.ReadFacilities(currentDoc);
                    break;
                case "Doors":
                    RevitDoors.ReadDoors(currentDoc);
                    break;
                case "Windows":
                default:
                case "Materials":
                    RevitMaterials.ReadMaterials(currentDoc);
                    break;
            }
            */
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class WriteDatabase : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            /*
            Document currentDoc = commandData.Application.ActiveUIDocument.Document;
            string combosetting = RevitDatabase.ReadComboBoxValue();
            switch(combosetting)
            {
                case "Project":
                    ProjectDataIO.UpdatePData(currentDoc);
                    break;
                case "Equipment":
                    RevitEquipment.WriteEquipment(currentDoc);
                    break;
                case "Facilities":
                    RevitFacilities.WriteFacilities(currentDoc);
                    break;
                case "Doors":
                    RevitDoors.WriteDoors(currentDoc);
                    break;
                case "Materials":
                default:
                    RevitMaterials.WriteMaterials(currentDoc);
                    break;
            }
            */
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class RevitNotes : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document currentDoc = commandData.Application.ActiveUIDocument.Document;
            using (Transaction trans = new Transaction(currentDoc, "Update Per Doc"))
            {
                trans.Start();
                Notes.EnterNotes(currentDoc);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
    #endregion
    public class RevitDatabase
    {
        private static string MyDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string ComboFile = MyDocs + "\\CC_Plugin_ComboFile.txt";
        public static string ReadComboBoxValue()
        {
            string ComboLine = "Null";
            if(File.Exists(ComboFile))
            {
                string[] lines = File.ReadAllLines(ComboFile);
                if(lines.Any(x => x.Contains("#ComboBox")))
                {
                    string line = lines.Where(x => x.Contains("#ComboBox")).Last();
                    {
                        if(line.Split('\t').Count() >1)
                        {
                            ComboLine = line.Split('\t')[1];
                        }
                    }
                }
            }
            return ComboLine;
        }
        public static void ChangeComboBoxValue(string newline)
        {
            string ComboLine = "#ComboBox\t" + newline;
            string[] line = new string[1] { ComboLine };
            File.WriteAllLines(ComboFile, line);
        }
        public static string GetFileName()
        {
            FileSaveDialog fsd = new FileSaveDialog("Text Files (*.txt)|*.txt");
            fsd.Title = "Database File";
            fsd.Show();
            ModelPath mp = fsd.GetSelectedModelPath();
            string filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
            return filename;
        }
        public static void createfile(Document currentDoc, int Category)
        {
            Application App = currentDoc.Application;
            App.SharedParametersFilename = DBIdentifiers.SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();
            ProjectInfo info = currentDoc.ProjectInformation;

            Param[] p;
            Parameter FileParam;

            string filename = GetFileName();
            switch(Category)
            {
                default:
                case ParamCategory.Materials:
                    p = ParameterGroup.MaterialTrackedParams;
                    FileParam = info.get_Parameter(ParameterLibrary.MaterialFile.ID);
                    break;
                case ParamCategory.Equipment:
                    p = ParameterGroup.EquipmentTrackedParams;
                    FileParam = info.get_Parameter(ParameterLibrary.EquipFile.ID);
                    break;
                case ParamCategory.Users:
                    FileParam = info.get_Parameter(ParameterLibrary.StaffFile.ID);
                    break;
                case ParamCategory.Doors:
                    FileParam = info.get_Parameter(ParameterLibrary.DoorFile.ID);
                    break;
            }

            if (!File.Exists(filename))
            {
                CC_CoreData.DatabaseIO.CreateFile(filename, Category);
            }
            
            FileParam.Set(filename);
        }
    }
}