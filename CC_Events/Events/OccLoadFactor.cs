using System.Collections.Generic;
using System.Linq;
using System;
using CC_Library.Parameters;
using CC_Plugin.Parameters;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Architecture;
using CC_Library.Predictions;

namespace CC_Plugin
{
    internal static class UpdateOccupantLoads
    {
        public static void synch(object sender, DocumentSynchronizingWithCentralEventArgs args)
        {
            Document doc = args.Document;
            using (TransactionGroup tg = new TransactionGroup(doc, "Update Occ Loads"))
            {
                tg.Start();
                List<Element> Rooms = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .ToList();
                foreach (Element e in Rooms)
                {
                    using (Transaction t = new Transaction(doc, "Update Occ Loads"))
                    {
                        t.Start();
                        e.UpdateOccLoads();
                        t.Commit();
                    }
                }
                tg.Commit();
            }
        }
        public static Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSynchronizingWithCentral += new EventHandler<DocumentSynchronizingWithCentralEventArgs>(synch);
            return Result.Succeeded;
        }
        public static Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentSynchronizingWithCentral -= new EventHandler<DocumentSynchronizingWithCentralEventArgs>(synch);
            return Result.Succeeded;
        }
        internal static void UpdateOccLoads(this Element e)
        {
            try
            {
                Room r = e as Room;
                double lf = double.Parse(e.GetElementParam(RoomParams.OccupantLoadFactor).Split(' ').First());
                double Area = r.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
                int load = 0;
                if (lf > 0)
                    load = (int)Math.Ceiling(Area / lf);
                r.SetIntParam(RoomParams.OccupantLoad, load.ToString());
            }
            catch { }
        }
    }
}
