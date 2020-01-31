using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System;
using Autodesk.Revit.UI;
using CC_Library;

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
                CommandLibrary.Transact(new CommandLibrary.DocStringCommand(IDParam.Set), doc);
                string Fam = CommandLibrary.Transact(new CommandLibrary.StringBasedDocCommand(FamParam.Set), doc,
                    args.PathName.Split('.').First().Split('\\').Last());
                if(doc.IsFamilyDocument && !MFConfirmParam.Get(doc))
                    CommandLibrary.Transact(new CommandLibrary.StringBasedDocCommand(MFParam.Set), doc,
                        TitleAnalysisPrediction.GenPrediction 
                        (args.PathName.Split('.').First().Split('\\').Last()).ToString());
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
