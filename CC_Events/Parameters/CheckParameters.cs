using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

/// <summary>
/// On Document startup
/// 1) Collect a list of parameters in th project by GUID.
/// 2) Cross check the list against a txt file, tracking - name, date added, guid, Object Categories?
/// 3) save back as a tab dlimittd text file (or csv file?)
/// 4) leave open column one. IF column 1.ToUpper = "DELETE", remove the parameter from the set and the txt file.
/// </summary>

namespace CC_Plugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ParamCheck : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            CheckParameters.Execute(doc);
            return Result.Succeeded;
        }
    }
    public class CheckParameters
    {
        class ProjectParameterData
        {
            public Definition Definition = null;
            public ElementBinding Binding = null;
            public string Name = null;                // Needed because accsessing the Definition later may produce an error.
            public bool IsSharedStatusKnown = false;  // Will probably always be true when the data is gathered
            public bool IsShared = false;
            public string GUID = null;
            public string type = null;
            public List<string> cats = new List<string>();
        }
        private static List<ProjectParameterData> GetPPD(Document doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }

            if (doc.IsFamilyDocument)
            {
                throw new Exception("doc cannot be family document.");
            }

            List<ProjectParameterData> data = new List<ProjectParameterData>();

            BindingMap map = doc.ParameterBindings;
            DefinitionBindingMapIterator it = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                ProjectParameterData ppd = new ProjectParameterData();
                ppd.Definition = it.Key;
                ppd.Name = it.Key.Name;
                ppd.Binding = it.Current as ElementBinding;
                ppd.type = it.Key.ParameterType.ToString();
                data.Add(ppd);
            }
            return data;
        }
        static bool AddProjectParameterBinding(
            Document doc,
            ProjectParameterData ppd,
            Category cat)
        {
            if (doc == null) { throw new ArgumentNullException("doc"); }
            if (doc.IsFamilyDocument) { throw new Exception("doc cannot be family document."); }
            if (ppd == null) { throw new ArgumentNullException("projectParameterData"); }
            if (cat == null) { throw new ArgumentNullException("category"); }

            bool result = false;

            CategorySet cats = ppd.Binding.Categories;

            if (cats.Contains(cat))
            {
                // It's already bound to the desired category. Nothing to do.
                string errorMessage = string.Format(
                  "The project parameter '{0}' is already bound to the '{1}' category.",
                  ppd.Definition.Name,
                  cat.Name);

                throw new Exception(errorMessage);
            }

            cats.Insert(cat);

            // See if the parameter is an instance or type parameter.

            InstanceBinding instanceBinding = ppd.Binding as InstanceBinding;

            if (instanceBinding != null)
            {
                InstanceBinding newInstanceBinding = doc.Application.Create.NewInstanceBinding(cats);

                if (doc.ParameterBindings.ReInsert(
                  ppd.Definition,
                  newInstanceBinding))
                {
                    result = true;
                }
            }
            else
            {
                TypeBinding typeBinding = doc.Application.Create.NewTypeBinding(cats);

                if (doc.ParameterBindings.ReInsert(ppd.Definition, typeBinding))
                {
                    result = true;
                }
            }
            return result;
        }
        static void PopulateProjectParameterData(
            Parameter par,
            ProjectParameterData PPD)
        {
            if (par == null) { throw new ArgumentNullException("parameter"); }
            if (PPD == null) { throw new ArgumentNullException("projectParameterDataToFill"); }

            PPD.IsSharedStatusKnown = true;
            PPD.IsShared = par.IsShared;
            if (par.IsShared)
            {
                if (par.GUID != null)
                {
                    PPD.GUID = par.GUID.ToString();
                }
            }
        }
        public static void Execute(Document doc)
        {
            Element projectInfoElement
              = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ProjectInformation)
                .FirstElement();

            Element firstWallTypeElement
              = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsElementType()
                .FirstElement();

            CategorySet categories = null;
            Parameter foundParameter = null;

            List<ProjectParameterData> projectParametersData
              = GetPPD(doc);

            foreach (ProjectParameterData projectParameterData
              in projectParametersData)
            {
                if (projectParameterData.Definition != null)
                {
                    categories = projectParameterData.Binding.Categories;
                    if (!categories.Contains(projectInfoElement.Category))
                    {
                        using (Transaction tempTransaction
                          = new Transaction(doc))
                        {
                            tempTransaction.Start("Temporary");

                            if (AddProjectParameterBinding(
                              doc, projectParameterData,
                              projectInfoElement.Category))
                            {
                                foundParameter
                                  = projectInfoElement.get_Parameter(
                                    projectParameterData.Definition);

                                if (foundParameter == null)
                                {
                                    if (!categories.Contains(
                                      firstWallTypeElement.Category))
                                    {
                                        if (AddProjectParameterBinding(
                                          doc, projectParameterData,
                                          firstWallTypeElement.Category))
                                        {
                                            foundParameter
                                              = firstWallTypeElement.get_Parameter(
                                                projectParameterData.Definition);
                                        }
                                    }
                                    else
                                    {
                                        // The project parameter was already 
                                        // bound to the Walls category.
                                        foundParameter
                                          = firstWallTypeElement.get_Parameter(
                                            projectParameterData.Definition);
                                    }

                                    if (foundParameter != null)
                                    {
                                        PopulateProjectParameterData(
                                          foundParameter,
                                          projectParameterData);
                                    }
                                    else
                                    {
                                        projectParameterData.IsSharedStatusKnown
                                          = false;  // Throw exception?
                                    }
                                }
                                else
                                {
                                    PopulateProjectParameterData(
                                      foundParameter,
                                      projectParameterData);
                                }
                            }
                            else
                            {
                                projectParameterData.IsShared = false;
                                projectParameterData.IsSharedStatusKnown = true;
                            }
                            tempTransaction.RollBack();
                        }
                    }
                    else
                    {
                        foundParameter
                          = projectInfoElement.get_Parameter(
                            projectParameterData.Definition);

                        if (foundParameter != null)
                        {
                            PopulateProjectParameterData(
                              foundParameter, projectParameterData);
                        }
                        else
                        {
                            projectParameterData.IsSharedStatusKnown
                              = false;  // Throw exception?
                        }
                    }

                }  // Whether or not the Definition object could be found

            }

            List<string> lines = new List<string>();

            foreach (ProjectParameterData projectParameterData
              in projectParametersData)
            {
                string s = "";
                s += projectParameterData.Name;
                s += "\t";

                if (projectParameterData.IsSharedStatusKnown)
                {
                    s += projectParameterData.IsShared.ToString();
                }
                else
                {
                    s += "<Unknown>";
                }

                if (projectParameterData.IsSharedStatusKnown &&
                    projectParameterData.IsShared)
                {
                    s += "\t";
                    s += projectParameterData.GUID;
                }
                else
                {
                    s += "\t";
                    s += "<Unknown<";
                }
                if(projectParameterData.type != null)
                {
                    s += "\t";
                    s += projectParameterData.type;
                }    
                lines.Add(s);
            }
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = "Select a text file",
                Title = "Save txt file"
            })
            {
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(sfd.FileName, lines);
                }
            }
        }
    }
}
