using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace DataAnalysis
{
    public class FamilyData
    {
        public string ID {get;}
        public Dictionary<string, int> Uses {get; set;}
        public Dictionary<string, int> Previous {get; set;}
        public List<string> Projects {get; set;}
        public int TotalUses { get; set; }
        public long AverageTime {get; set;}
        
        public FamilyData(string id)
        {
            this.ID = id;
            this.Uses = new Dictionary<string, int>();
            this.Previous = new Dictionary<string, int>();
            this.Projects = new List<string>();
            this.TotalUses = 0;
            this.AverageTime = 0;
        }
        public void Write(string folder)
        {
        }
    }
    public class Datasort
    {
        public delegate bool DataQualifier(XDocument xdoc);
        public static void Qualify(string InFolder, string OutFolder, int Days)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            List<string> lines = new List<string>();
            string[] files = Directory.GetFiles(InFolder);
            var Data = new List<FamilyData>();
            
            foreach (string f in files)
            {
                try
                {
                    XDocument doc = XDocument.Load(f);
                    string id = doc.Root.Attribute("EleID").Value;
                    if(id != null)
                    {
                        FamilyData fd;
                        if(Data.Any(x => x.ID == id))
                            fd = Data.Where(x => x.ID == id).First();
                        else
                            fd = new FamilyData(id);
                        if(doc.Root.Attribute("PrjID") != null)
                            if(!fd.Projects.Contains(doc.Root.Attribute("PrjID").Value)
                                fd.Projects.Add(doc.Root.Attribute("PrjID").Value;
                    }
                }
            }
            foreach(FamilyData fd in Data)
            {
                fd.Write(OutFolder);
            }
        }
        public static void Count(string folder, string file, int Days, DataQualifier qual)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            List<string> lines = new List<string>();
            string[] files = Directory.GetFiles(folder);
            Dictionary<string, int[]> Data = new Dictionary<string, int[]>();

            foreach (string f in files)
            {
                try
                {
                    XDocument doc = XDocument.Load(f);
                    if (qual(doc))
                    {
                        string id = doc.Root.Attribute("EleID").Value;
                        DateTime date;
                        DateTime.TryParseExact(doc.Root.Attribute("PlaceTime").Value, "yyyyMMddhhmmss", 
                            enUS, DateTimeStyles.None, out date);
                        int e = (DateTime.Today - date).Days;
                        if (e < Days)
                        {
                            if (Data.ContainsKey(id))
                            {
                                Data[id][e]++;
                            }
                            else
                            {
                                int[] a = new int[Days];
                                a[e]++;
                                Data.Add(id, a);
                            }
                        }
                    }
                }
                catch { }
            }

            string s = string.Empty;
            foreach(var kvp in Data)
            {
                s += kvp.Key + ",";
            }
            s.TrimEnd(',');
            lines.Add(s);
            for(int i = 0; i < Days; i++)
            {
                string t = string.Empty;
                foreach(var kvp in Data)
                {
                    t += kvp.Value[i] + ",";
                }
                t.TrimEnd(',');
                lines.Add(t);
            }
            File.WriteAllLines(file, lines);
        }
        public static void Average(string folder, string file, string Param, DataQualifier qual)
        {
            string[] files = Directory.GetFiles(folder);
            Dictionary<string, double[]> Data = new Dictionary<string, double[]>();

            foreach (string f in files)
            {
                try
                {
                    XDocument doc = XDocument.Load(f);
                    if (qual(doc))
                    {
                        string id = doc.Root.Attribute("EleID").Value;
                        double.TryParse(doc.Root.Attribute(Param).Value, out double x);
                        if (!double.IsNaN(x))
                        {
                            if (Data.ContainsKey(id))
                            {
                                Data[id][0] += x;
                                Data[id][1]++;
                            }
                            else
                            {
                                Data.Add(id, new double[2] { x, 1 });
                            }
                        }
                    }
                }
                catch { }
            }

            List<string> lines = new List<string>();
            foreach (var kvp in Data)
            {
                double y = kvp.Value[0] / kvp.Value[1];
                string s = kvp.Key + "," + y.ToString();
                lines.Add(s);
            }
            File.WriteAllLines(file, lines);
        }
        public static void Compare(string folder, string file, string[] parameters, DataQualifier qual)
        {
            string[] files = Directory.GetFiles(folder);
            Dictionary<string[], int> Data = new Dictionary<string[], int>();

            foreach (string f in files)
            {
                try
                {
                    XDocument doc = XDocument.Load(f);
                    if (qual(doc))
                    {
                        bool x = true;
                        string[] pars = new string[parameters.Count()];
                        for (int i = 0; i < parameters.Count(); i++)
                        {
                            if (doc.Root.Attribute(parameters[i]) != null)
                            {
                                pars[i] = parameters[i] + "," + doc.Root.Attribute(parameters[i]).Value;
                            }
                            else
                            {
                                x = false;
                                break;
                            }
                        }
                        if (x)
                        {
                            if (Data.ContainsKey(pars))
                                Data[pars]++;
                            else
                                Data.Add(pars, 1);
                        }
                    }
                }
                catch { }
            }

            List<string> lines = new List<string>();
            foreach (var kvp in Data)
            {
                string[] y = kvp.Key;
                string s = string.Empty;
                foreach (string z in y)
                {
                    s += z + ",";
                }
                s += kvp.Value;
                lines.Add(s);
            }
            File.WriteAllLines(file, lines);
        }
        public static void Variance(string folder, string file, string param, DataQualifier qual)
        {
            string[] files = Directory.GetFiles(folder);
            Dictionary<string, Dictionary<string, int>> Data = new Dictionary<string, Dictionary<string, int>>();

            foreach (string f in files)
            {
                try
                {
                    XDocument doc = XDocument.Load(f);
                    if (qual(doc))
                    {
                        string id = doc.Root.Attribute("EleID").Value;
                        string p = doc.Root.Attribute(param).Value;
                        if (Data.ContainsKey(id))
                        {
                            if (Data[id].ContainsKey(param))
                                Data[id][param]++;
                            else
                                Data[id].Add(param, 1);
                        }
                        else
                        {
                            Dictionary<string, int> d = new Dictionary<string, int>();
                            d.Add(param, 1);
                            Data.Add(id, d);
                        }
                    }
                }
                catch { }
            }

            List<string> lines = new List<string>();
            foreach (var x in Data)
            {
                string s = x.Key;
                foreach (var y in x.Value)
                {
                    s += "," + y.Key + " : " + y.Value.ToString();
                }
                lines.Add(s);
            }
            File.WriteAllLines(file, lines);
        }
    }
}
