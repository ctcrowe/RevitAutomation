using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CC_Plugin
{
    internal class ProjectTime
    {
        private static string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string subdir = dir + "\\CC_TimeData";
        private static string timefile = subdir + "\\TimeData.txt";

        public static string Get(string id)
        {
            if (File.Exists(subdir + "\\" + id + ".xml"))
            {
                XDocument doc = XDocument.Load(subdir + "\\" + id + ".xml");
                if (doc.Root.Attribute("TOTAL") != null)
                    return doc.Root.Attribute("TOTAL").Value;
            }
            return null;
        }
        private static XDocument SetupDoc(string id)
        {
            if (File.Exists(subdir + "\\" + id + ".xml"))
                return XDocument.Load(subdir + "\\" + id + ".xml");
            else
            {
                return new XDocument(new XElement("TIMEDATA"))
                { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            }
        }
        public static void Run(Document doc)
        {
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);
            DateTime e = DateTime.Now;
            if (File.Exists(timefile))
            {
                DateTime.TryParse(File.ReadAllLines(timefile).First().Split('\t')[1], out DateTime t);
                string id = File.ReadAllLines(timefile).First().Split('\t')[0];
                TimeSpan ts = t - e;
                double f = Math.Abs(ts.TotalHours);
                UpdateTime(id, f);
            }
            /*string mark = IDParam.Get(doc) + "\t" + e;
            List<string> timemark = new List<string>() { mark };
            File.WriteAllLines(timefile, timemark);*/
        }
        public static void Run()
        {
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);
            DateTime e = DateTime.Now;
            if (File.Exists(timefile))
            {
                DateTime.TryParse(File.ReadAllLines(timefile).First().Split('\t')[1], out DateTime t);
                string id = File.ReadAllLines(timefile).First().Split('\t')[0];
                TimeSpan ts = t - e;
                double f = Math.Abs(ts.TotalHours);
                UpdateTime(id, f);
            }
        }
        private static void UpdateTime(string id, double t)
        {
            XDocument doc = SetupDoc(id);

            if (doc.Root.Attribute("TOTAL") == null)
            {
                doc.Root.Add(new XAttribute("TOTAL", t.ToString()));
            }
            else
            {
                double.TryParse(doc.Root.Attribute("TOTAL").Value, out double ttime);
                double total = ttime + t;
                doc.Root.Attribute("TOTAL").Value = total.ToString();
            }
            if (!doc.Root.Elements("DAY").Any(x => x.Attribute("DATE").Value == DateTime.Today.ToString("yyyyMMdd")))
            {
                XElement e = new XElement("DAY");
                e.Add(new XAttribute("DATE", DateTime.Today.ToString("yyyMMdd")));
                e.Add(new XAttribute("TIME", t.ToString()));
                doc.Root.Add(e);
            }
            else
            {
                XElement e = doc.Root.Elements("DAY").Where(x => x.Attribute("DATE").Value == DateTime.Today.ToString("yyyyMMdd")).First();
                double.TryParse(e.Attribute("TIME").Value, out double xtime);
                double daytime = xtime + t;
                e.Attribute("TIME").Value = daytime.ToString();
            }
            doc.Save(subdir + "\\" + id + ".xml");
        }
        public static void End()
        {
            if (File.Exists(timefile))
                File.Delete(timefile);
        }
    }
}
