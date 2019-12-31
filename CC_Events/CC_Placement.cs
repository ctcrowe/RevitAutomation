using System;
using Autodesk.Revit.DB;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace CC_Plugin
{
    internal class Placement
    {
        public static void Get(Document doc, XmlLib lib, ElementId eid)
        {
            try
            {
                FamilyInstance inst = doc.GetElement(eid) as FamilyInstance;
                if (inst != null)
                {
                    XElement xdoc = lib.GetElement(IDParam.Get(inst), "FAMILY");
                    XElement place = new XElement("PLACEMENT");
                    place.Add(new XAttribute("TIME", DateTime.Now.ToString("yyyyMMddhhmmss")));
                    place.Add(new XAttribute("VIEW", doc.ActiveView.Name));
                    place.Add(new XAttribute("SCALE", doc.ActiveView.Scale.ToString()));
                    place.Add(new XAttribute("CATEGORY", inst.Category.Name));
                    //place.Add(new XAttribute("HOURS", xdoc.Attribute("TOTAL TIME").Value));
                    if (!doc.IsFamilyDocument)
                        if (inst.Room != null)
                            place.Add(new XAttribute("ROOM", inst.Room.Name));
                    xdoc.Add(place);
                }
            }
            catch { }
        }
        public static void DailyCount(string file)
        {
            XmlLib lib = new XmlLib();
            List<string> datatable = new List<string>();
            List<XElement> fams = lib.GetElements("FAMILY").ToList();
            Dictionary<string, int[]> data = new Dictionary<string, int[]>();
            for (int i = 0; i < fams.Count(); i++)
            {
                List<XElement> Placements = fams[i].Elements("PLACEMENT").ToList();
                foreach (XElement p in Placements)
                {
                    string day = p.Attribute("TIME").Value.Substring(0, 7);
                    if (data.ContainsKey(day))
                        data[day][i]++;
                    else
                    {
                        data.Add(day, new int[fams.Count()]);
                        data[day][i] = 1;
                    }
                }
            }
            var list = data.Keys.ToList();
            list.Sort();

            string title = "";
            foreach (var key in list)
            {
                title += "\t" + data[key];
            }
            datatable.Add(title);

            for (int i = 0; i < fams.Count(); i++)
            {
                string s = fams[i].Attribute("ID").Value;
                foreach (var key in list)
                {
                    s += "\t" + data[key][i].ToString();
                }
                datatable.Add(s);
            }
            File.WriteAllLines(file, datatable);
        }
        public static void ViewScale(string file)
        {
            XmlLib lib = new XmlLib();
            List<string> datatable = new List<string>();
            List<XElement> fams = lib.GetElements("FAMILY").ToList();
            Dictionary<string, int[]> data = new Dictionary<string, int[]>();
            for (int i = 0; i < fams.Count(); i++)
            {
                List<XElement> Placements = fams[i].Elements("PLACEMENT").ToList();
                foreach (XElement p in Placements)
                {
                    string scale = p.Attribute("SCALE").Value;
                    if (data.ContainsKey(scale))
                        data[scale][i]++;
                    else
                    {
                        data.Add(scale, new int[fams.Count()]);
                        data[scale][i] = 1;
                    }
                }
            }
            var list = data.Keys.ToList();
            list.Sort();

            string title = "";
            foreach (var Key in list)
            {
                title += "\t" + Key;
            }
            datatable.Add(title);

            for (int i = 0; i < fams.Count(); i++)
            {
                string s = fams[i].Attribute("ID").Value;
                foreach (var Key in list)
                {
                    s += "\t" + data[Key][i].ToString();
                }
                datatable.Add(s);
            }
            File.WriteAllLines(file, datatable);
        }
    }
}