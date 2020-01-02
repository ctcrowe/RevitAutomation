﻿using System;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System.Xml.Linq;

namespace CC_Plugin
{
    internal class DocSynching
    {
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
        public static void synch(object sender, DocumentSynchronizingWithCentralEventArgs args)
        {
            Document doc = args.Document;
            using (TranscationGroup tg = new TransactionGroup(doc, "Doc Synching"))
            {
                tg.Start();
                using (Transaction t = new Transaction(doc, "Room Data"))
                {
                    t.Start();
                    CollectRoomData(doc);
                    t.Commit();
                }
                using (Transaction t = new Transaction(doc, "Add Families"))
                {
                    t.Start();
                    EmbeddedFamilies.run(doc, "Symbols");
                    t.Commit();
                }
                tg.Commit();
            }
        }
        public static void CollectRoomData(Document doc)
        {
            List<Element> InstCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).ToElements().ToList();
            List<Element> RoomCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToElements().ToList();
            
            var Data =  new Dictionary<string, List<string>>();
            
            foreach(Element e in RoomCollector)
            {
                Room r = e as Room;
                List<string> l = new List<string>();
                Data.Add(r.Name, l);
            }
            foreach(Element e in InstCollector)
            {
                FamilyInstance f = e as FamilyInstance;
                string id = IDParam.Get(f);
                if(id != null)
                {
                    if(f.Room != null)
                    {
                        if(Data.ContainsKey(f.Room.Name))
                            Data[f.Room.Name].Add(id);
                        else
                        {
                            List<string> l = new List<string>();
                            l.Add(id)
                            Data.Add(f.Room.Name, l);
                        }
                    }
                    else
                    {
                        if(f.FromRoom != null)
                        {
                            if(Data.ContainsKey(f.FromRoom.Name))
                                Data[f.FromRoom.Name].Add(id);
                            else
                            {
                                List<string> l = new List<string>();
                                l.Add(id)
                                Data.Add(f.FromRoom.Name, l);
                            }
                        }
                    }
                }
            }
            XDocument xdoc = new XDocument(new XElement(IDParam.Get(doc))) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            if (data.Keys.Count > 0)
            {
                foreach (var kvp in Data)
                {
                    XElement ele = new XElement(kvp.Key);
                    foreach(string s in kvp.Value)
                    {
                        XElement e = new XElement(s);
                        ele.Add(e);
                    }
                    xdoc.Root.Add(ele);
                }
                xdoc.Save(fn);
            }
        }
    }
}
