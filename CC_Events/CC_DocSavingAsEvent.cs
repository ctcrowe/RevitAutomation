using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    internal class DocSavingAs
    {
        public static void Event(object sender, DocumentSavingAsEventArgs args)
        {
            Document doc = args.Document;
            using (TransactionGroup tg = new TransactionGroup(doc, "Saving Transactions"))
            {
                tg.Start();
                using (Transaction t = new Transaction(doc, "Saving Transaction"))
                {
                    t.Start();
                    string id = IDParam.Set(doc);
                    string Fam = FamParam.Set(doc, args.PathName.Split('.').First().Split('\\').Last());
                    t.Commit();
                }
                if(doc.IsFamilyDocument)
                {
                    if(!MFConfirmParam.Get())
                    {
                        using (Transaction t = new Transaction(doc, "MF Transaction"))
                        {
                            t.Start();
                            MFParam.Set
                            (
                                TitleAnalysisPrediction
                                .GenPrediction
                                (
                                    args
                                    .PathName
                                    .Split('.')
                                    .First()
                                    .Split('\\')
                                    .Last()
                                )
                            );
                            t.Commit();
                        }
                    }
                }
                tg.Commit();
            }
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSavingAs += new EventHandler<DocumentSavingAsEventArgs>(Event);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSavingAs -= new EventHandler<DocumentSavingAsEventArgs>(Event);
            return Result.Succeeded;
        }
    }
}
