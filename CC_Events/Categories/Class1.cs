using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using CC_Library;
using CC_Library.Predictions;

namespace CC_Plugin
{
    internal static class CMD_PredictLineWeights
    {
        public static void PullLineWeights(this View v)
        {
            Document doc = v.Document;
            foreach(Category c in doc.Settings.Categories)
            {
                if(c.CategoryType == CategoryType.Model && c.CanAddSubcategory)
                {
                    try
                    {
                        var COverrides = v.GetCategoryOverrides(c.Id);
                        var CProj = COverrides.ProjectionLineWeight > 0 ?
                            COverrides.ProjectionLineWeight :
                            c.GetLineWeight(GraphicsStyleType.Projection);
                        typeof(ProjectionLineWeightNetwork).CreateEmbed(c.Name, CProj.ToString(), v.Name);
                    }
                    catch {}
                    foreach(Category sc in c.SubCategories)
                    {
                        try
                        {
                            var CSOverrides = v.GetCategoryOverrides(cs.Id);
                            var CsProj = CSOverrides.ProjectionLineWeight > 0 ?
                                CSOverrides.ProjectionLineWeight :
                                cs.GetLineWeight(GraphicsStyleType.Projection);
                            typeof(ProjectionLineWeightNetwork).CreateEmbed(c.Name + "_" + sc.Name, , v.Name);
                        }
                        catch {}
                    }
                }
            }
        }
        public static void PredictLineWeights(this View v)
        {
            using (Transaction t = new Transaction(v.Document, "Overwrite Line Weights"))
            {
                t.Start();
                Document doc = v.Document;
                var Alpha = new Alpha("ViewName", CMDLibrary.WriteNull);
                var ViewLoc = Alpha.Forward(v.Name);
                foreach (Category c in doc.Settings.Categories)
                {
                    if (c.CategoryType == CategoryType.Model && c.CanAddSubcategory)
                    {
                        try
                        {
                            var COverrides = v.GetCategoryOverrides(c.Id);
                            var CPrediction = ProjectionLineWeightNetwork.Predict(c.Name, CMDLibrary.WriteNull, ViewLoc);
                            COverrides.SetProjectionLineWeight(CPrediction.ToList().IndexOf(CPrediction.Max()) + 1);
                            v.SetCategoryOverrides(c.Id, COverrides);
                            typeof(ProjectionLineWeightNetwork).CreateEmbed(c.Name, CPrediction.ToList().IndexOf(CPrediction.Max()).ToString(), v.Name);
                        }
                        catch { }
                        foreach (Category sc in c.SubCategories)
                        {
                            try
                            {
                                var SCOverrides = v.GetCategoryOverrides(sc.Id);
                                var SCPrediction = ProjectionLineWeightNetwork.Predict(c.Name + "_" + sc.Name, CMDLibrary.WriteNull);
                                SCOverrides.SetProjectionLineWeight(SCPrediction.ToList().IndexOf(SCPrediction.Max()) + 1);
                                v.SetCategoryOverrides(sc.Id, SCOverrides);
                                typeof(ProjectionLineWeightNetwork).CreateEmbed(c.Name + "_" + sc.Name, SCPrediction.ToList().IndexOf(SCPrediction.Max()).ToString(), v.Name);
                            }
                            catch { }
                        }
                    }
                }

                t.Commit();
            }
        }
    }
    public class LineweightTab
    {
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Plugin.dll";
            return dll;
        }
        public static void LWTab(UIControlledApplication uiApp, string tabname)
        {
            RibbonPanel CPanel = uiApp.CreateRibbonPanel(tabname, "Line Weights");

            PushButtonData B1 = new PushButtonData(
                "Set Line Weights",
                "Set Line Weights",
                dllpath(),
                "CC_Plugin.Weights");

            var Button = CPanel.AddItem(B1) as PushButton;

            PushButtonData B2 = new PushButtonData(
                "Pull Predictions",
                "Pull Predictions",
                dllpath(),
                "CC_Plugin.PullPredictions");

            var Button2 = CPanel.AddItem(B2) as PushButton;
            
            PushButtonData B3 = new PushButtonData(
                "Run Predictions",
                "Run Predictions",
                dllpath(),
                "CC_Plugin.RunPredictions");
            var but3 = CPanel.AddItem(B3) as PushButton;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Weights : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var view = commandData.Application.ActiveUIDocument.Document.ActiveView;
            view.PredictLineWeights();
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class PullPredictions : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var view = commandData.Application.ActiveUIDocument.Document.ActiveView;
            view.PullLineWeights();
            return Result.Succeeded;
        }
    }
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class RunPredictions : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Datasets.RunPredictions(CMDLibrary.WriteNull, 5);
            return Result.Succeeded;
        }
    }
}
