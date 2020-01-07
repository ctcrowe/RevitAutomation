using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Linq;
using System.Xml.Linq;

namespace CC_Plugin
{
    internal class DocSynching
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string dir = directory + "\\CC_PrjData";
        
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
            using (TransactionGroup tg = new TransactionGroup(doc, "Doc Synching"))
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
            
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            var Data =  new Dictionary<string, List<string>>();
            Data.Add("EXTERIOR", new List<string>());
            foreach(Element e in RoomCollector)
            {
                Room r = e as Room;
                List<string> l = new List<string>();
                Data.Add(r.Name, l);
            }
            foreach(Element e in InstCollector)
            {
            	try
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
                            l.Add(id);
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
                                l.Add(id);
                                Data.Add(f.FromRoom.Name, l);
                            }
                        }
                        else
                        {
                            Data["EXTERIOR"].Add(id);
                        }
                    }
                }
            	}
            	catch{}
            }
            string docID = IDParam.Get(doc);
            if(docID != null)
            {
            	XDocument xdoc = new XDocument(new XElement("PROJECT")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                xdoc.Root.Add(new XAttribute("TIME", DateTime.Now.ToString("yyyyMMddhhmmss")));
                xdoc.Root.Add(new XAttribute("ID", docID));
                if (Data.Keys.Count > 0)
                {
                    foreach (var kvp in Data)
                    {
                    	XElement ele = new XElement(kvp.Key.Replace(' ', '_').Replace("\'", ""));
                        foreach(string s in kvp.Value)
                        {
                            XElement e = new XElement("ELEMENT");
                            e.Add(new XAttribute("ID", s));
                            ele.Add(e);
                        }
                        xdoc.Root.Add(ele);
                    }
                }
                xdoc.Save(dir + "\\" + docID + ".xml");
            }
        }
    }
}
