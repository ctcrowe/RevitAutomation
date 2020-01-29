using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;

using System;
using System.Reflection;
using System.IO;

namespace CC_Plugin
{
    internal class ViewChanged
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static Result OnStartup(UIControlledApplication app)
        {
            app.ViewActivated += new EventHandler<Autodesk.Revit.UI.Events.ViewActivatedEventArgs>(Event);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ViewActivated -= new EventHandler<Autodesk.Revit.UI.Events.ViewActivatedEventArgs>(Event);
            ProjectTime.Run();
            ProjectTime.End();
            return Result.Succeeded;
        }
        public static void Event(object sender, ViewActivatedEventArgs args)
        {
            Document doc = args.Document;
            using (TransactionGroup tg = new TransactionGroup(doc, "Transaction"))
            {
                tg.Start();
                using (Transaction t = new Transaction(doc, "trans 1"))
                {
                    t.Start();
                    ProjectTime.Run(doc);
                    t.Commit();
                }
                tg.Commit();
            }
        }
    }
}