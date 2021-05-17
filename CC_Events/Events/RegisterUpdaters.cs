using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using CC_Library.Parameters;
using CC_Plugin.Parameters;


namespace CC_Plugin
{
    internal class RegUpdaters
    {
        public static void DocOpened(object sender, DocumentOpenedEventArgs args)
        {
            UpdateOccLoad.ProjectStartup(args.Document.Application.ActiveAddInId, args.Document);
        }
        public static void DocClosed(object sender, DocumentClosingEventArgs args)
        {
            UpdateOccLoad.ProjectShutdown(args.Document.Application.ActiveAddInId, args.Document);
        }
        public static void DocCreated(object sender, DocumentCreatedEventArgs args)
        {
            UpdateOccLoad.ProjectStartup(args.Document.Application.ActiveAddInId, args.Document);
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(DocOpened);
            app.ControlledApplication.DocumentClosing += new EventHandler<DocumentClosingEventArgs>(DocClosed);
            app.ControlledApplication.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(DocCreated);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(DocOpened);
            app.ControlledApplication.DocumentClosing -= new EventHandler<DocumentClosingEventArgs>(DocClosed);
            app.ControlledApplication.DocumentCreated -= new EventHandler<DocumentCreatedEventArgs>(DocCreated);
            return Result.Succeeded;
        }
    }
}