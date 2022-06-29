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
        public static void RunLineWeightPredictions(this View v)
        {
            if (v.ViewTemplateId != ElementId.InvalidElementId)
                v = v.Document.GetElement(v.ViewTemplateId) as View;
            Document doc = v.Document;
            List<string> data1 = new List<string>();
            foreach(Category c in doc.Settings.Categories)
            {
                if(c.CategoryType == CategoryType.Model && c.CanAddSubcategory)
                {
                    try
                    {
                        var COverrides = v.GetCategoryOverrides(c.Id);
                        var CProj = COverrides.ProjectionLineWeight > 0 ?
                            COverrides.ProjectionLineWeight :
                            c.GetLineWeight(GraphicsStyleType.Projection) > 0 ?
                                c.GetLineWeight(GraphicsStyleType.Projection) : 1;
                        data1.Add("ProjectionLineWeightNetwork," + c.Name + "," + v.Name + "," + CProj.ToString());
                        typeof(LineWeightNetwork).CreateEmbed(c.Name, CProj.ToString(), v.Name);
                    }
                    catch { }
                    foreach (Category sc in c.SubCategories)
                    {
                        try
                        {
                            var CSOverrides = v.GetCategoryOverrides(sc.Id);
                            var CsProj = CSOverrides.ProjectionLineWeight > 0 ?
                                CSOverrides.ProjectionLineWeight :
                                sc.GetLineWeight(GraphicsStyleType.Projection) > 0 ?
                                    sc.GetLineWeight(GraphicsStyleType.Projection) : 1;
                            data1.Add("ProjectionLineWeightNetwork," + c.Name + "_" + sc.Name + "," + v.Name + "," + CsProj.ToString());
                            typeof(LineWeightNetwork).CreateEmbed(c.Name + "_" + sc.Name, CsProj.ToString(), v.Name);
                        }
                        catch { }
                    }
                }
            }
            Random r = new Random();
            var dataset1 = data1.OrderBy(x => r.NextDouble()).Take(16).ToArray();
            var Xfmr = Transformers.ProjectionLineWeightTransformer(CMDLibrary.WriteNull);
            var Cut = Transformers.CutLineWeightTransformer(CMDLibrary.WriteNull);
            var Alpha1 = Transformers.ProjectionLineWeightAlpha1(CMDLibrary.WriteNull);
            var Alpha2 = Transformers.ViewNameAlpha(CMDLibrary.WriteNull);
            var error = new double[2];
            for (int i = 0; i < 5; i++)
            {
                var e = LineWeightNetwork.Propogate(
                                    dataset1,
                                    CMDLibrary.WriteNull,
                                    Xfmr,
                                    Alpha1,
                                    Alpha2);
                error[0] += e[0];
                error[1] += e[1];
            }
            error[0] /= 5;
            error[1] /= 5;
            TaskDialog.Show("Network Training", "Average Error Was : " + error[0] + "\r\nAccuracy Was : " + error[1]);
            CMDLibrary.Launch();
        }
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
                        typeof(LineWeightNetwork).CreateEmbed(c.Name, CProj.ToString(), v.Name);
                    }
                    catch {}
                    foreach(Category sc in c.SubCategories)
                    {
                        try
                        {
                            var CSOverrides = v.GetCategoryOverrides(sc.Id);
                            var CsProj = CSOverrides.ProjectionLineWeight > 0 ?
                                CSOverrides.ProjectionLineWeight :
                                sc.GetLineWeight(GraphicsStyleType.Projection);
                            typeof(LineWeightNetwork).CreateEmbed(c.Name + "_" + sc.Name, CsProj.ToString(), v.Name);
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

                var Xfmr = Transformers.ProjectionLineWeightTransformer(CMDLibrary.WriteNull);
                var XfmrAlpha = Transformers.ProjectionLineWeightAlpha1(CMDLibrary.WriteNull);
                var XfmrAlpha2 = Transformers.ViewNameAlpha(CMDLibrary.WriteNull);

                foreach (Category c in doc.Settings.Categories)
                {
                    if (c.CategoryType == CategoryType.Model && c.CanAddSubcategory)
                    {
                        try
                        {
                            var COverrides = v.GetCategoryOverrides(c.Id);

                            var CPrediction = LineWeightNetwork.Predict(
                                c.Name,
                                CMDLibrary.WriteNull,
                                Xfmr,
                                XfmrAlpha,
                                XfmrAlpha2,
                                v.Name);
                            COverrides.SetProjectionLineWeight(CPrediction.ToList().IndexOf(CPrediction.Max()) + 1);
                            v.SetCategoryOverrides(c.Id, COverrides);
                            typeof(LineWeightNetwork).CreateEmbed(c.Name, CPrediction.ToList().IndexOf(CPrediction.Max()).ToString(), v.Name);
                        }
                        catch { }
                        foreach (Category sc in c.SubCategories)
                        {
                            try
                            {
                                var SCOverrides = v.GetCategoryOverrides(sc.Id);
                                var SCPrediction = LineWeightNetwork.Predict(
                                    c.Name + "_" + sc.Name,
                                    CMDLibrary.WriteNull,
                                    Xfmr,
                                    XfmrAlpha,
                                    XfmrAlpha2,
                                    v.Name);
                                SCOverrides.SetProjectionLineWeight(SCPrediction.ToList().IndexOf(SCPrediction.Max()) + 1);
                                v.SetCategoryOverrides(sc.Id, SCOverrides);
                                typeof(LineWeightNetwork).CreateEmbed(c.Name + "_" + sc.Name, SCPrediction.ToList().IndexOf(SCPrediction.Max()).ToString(), v.Name);
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
            var view = commandData.Application.ActiveUIDocument.Document.ActiveView;
            view.RunLineWeightPredictions();
            //var error = Datasets.RunPredictions(CMDLibrary.WriteNull, 5);
            //TaskDialog.Show("Network Training", "Average Error Was : " + error[0] + "\r\nAccuracy Was : " + error[1]);
            return Result.Succeeded;
        }
    }
}
