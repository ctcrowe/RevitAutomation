using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

using CC_Plugin.Parameters;
using CC_Library.Parameters;
using CC_Library.Predictions;
using CC_Library;

namespace CC_Plugin
{
    internal static class MFSaveFamily
    {
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSaved += new EventHandler<DocumentSavedEventArgs>(SavedEvent);
            app.ControlledApplication.DocumentSavedAs += new EventHandler<DocumentSavedAsEventArgs>(SavedAsEvent);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSaved -= new EventHandler<DocumentSavedEventArgs>(SavedEvent);
            app.ControlledApplication.DocumentSavedAs -= new EventHandler<DocumentSavedAsEventArgs>(SavedAsEvent);
            return Result.Succeeded;
        }
        public static void SavedEvent(object sender, DocumentSavedEventArgs args)
        {
            SetFamily(args.Document.PathName, args.Document);
            SaveFamily(args.Document.PathName, args.Document);
        }
        public static void SavedAsEvent(object sender, DocumentSavedAsEventArgs args)
        {
            SetFamily(args.Document.PathName, args.Document);
            SaveFamily(args.Document.PathName, args.Document);
        }
        internal static string PredictMF(string message)
        {
            string sURL = ("https://us-west2-upheld-now-290121.cloudfunctions.net/PredictMasterformat?message=" + message);

            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (Task<HttpResponseMessage> response = client.GetAsync(sURL))
            using (HttpContent content = response.Result.Content)
            {
                Task<string> result = content.ReadAsStringAsync();
                return result.Result;
            }
        }
        public static void SetFamily(string fn, Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                var output = PredictMF(fn.Split('\\').Last().Split('.').First());
                using (TransactionGroup tg = new TransactionGroup(doc, "Set MF"))
                {
                    tg.Start();
                    using (Transaction t = new Transaction(doc, "Add MF Param"))
                    {
                        t.Start();
                        try { doc.AddParam(Params.Masterformat); }
                        catch (Exception e) { e.OutputError(); }
                        t.Commit();
                    }
                    if (doc.FamilyManager.Types.Size < 1)
                    {
                        using (Transaction t = new Transaction(doc, "Create Family Type"))
                        {
                            t.Start();
                            doc.FamilyManager.NewType("Automatic Type");
                            t.Commit();
                        }
                    }
                    using (Transaction t = new Transaction(doc, "Set MF Param"))
                    {
                        t.Start();
                        try { doc.SetFamilyParam(Params.Masterformat, output); }
                        catch (Exception e) { e.OutputError(); }
                        t.Commit();
                    }
                    tg.Commit();
                }
            }
        }
        public static void SaveFamily(string fn, Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                string prefix = "";
                switch(doc.OwnerFamily.FamilyCategoryId.IntegerValue)
                {
                    default:
                        prefix = "Ele";
                        break;
                    case (int)BuiltInCategory.OST_DetailComponents:
                        prefix = "Det";
                        break;
                    case (int)BuiltInCategory.OST_ProfileFamilies:
                        prefix = "Pro";
                        break;
                }            
                string folder = "CC_Families".GetMyDocs().GetDir();
                var Division = PredictMF(fn.Split('\\').Last().Split('.').First());
                typeof(MasterformatNetwork).CreateEmbed(fp.Split('\\').Last().Split('.').First(), Division);
                string SubDir = folder + "\\Division " + Division;
                if(!Directory.Exists(SubDir))
                    Directory.CreateDirectory(SubDir);
                
                string nf = !fn.Split('\\').Last().StartsWith(prefix + "_")?
                SubDir + "\\" + prefix + "_" + fn.Split('\\').Last().Split('.').First() + ".rfa":
                SubDir + "\\" + fn.Split('\\').Last().Split('.').First() + ".rfa";
                File.Copy(fn, nf, true);
            }
        }
    }
}
