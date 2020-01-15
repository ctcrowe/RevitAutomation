using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace DataAnalysis
{
    public class WordSections
    {
        public static List<string> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<string>();
            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        List<string> title = SplitTitle(ele);
                        foreach (string s in title)
                            if (!data.Contains(s))
                                data.Add(s);
                    }
                }
            }
            if(file.Exists(xfile))
            {
                XDocument doc = XDocument.Load(xfile);
                foreach(XElement ele in doc.Root.Elements())
                {
                    data.Add(ele.Attribute("Value").Value);
                }
            }
            return data;
        }
        public static void CopyToXml(string file, string n)
        {
            string[] lines = File.ReadAllLines(file);
            XDocument doc = new XDocument();
            foreach(string s in lines)
            {
                XElement e = new XElement("string");
                e.Add(new XAttribute("Value", s));
            }
            doc.Save(n);
        }
        public static List<string> SplitTitle(string s)
        {
            List<string> data = new List<string>();
            int b = 0;
            char[] cs = s.ToCharArray();
            for (int i = 1; i < cs.Count(); i++)
            {
                if (!char.IsLetter(cs[i]))
                {
                    if (i > b && b < )
                    {
                        string z = string.Empty;
                        for (int j = b; j < i; j++)
                        {
                            z += cs[j];
                        }
                        data.Add(z);
                    }
                    b = i + 1;
                }
                else
                {
                    if (char.IsUpper(cs[i]) && !char.IsUpper(cs[i - 1]))
                    {
                        if (i > b)
                        {
                            string z = string.Empty;
                            for (int j = b; j < i; j++)
                            {
                                z += cs[j];
                            }
                            data.Add(z);
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
        public static void run()
        {
            string filedir = string.Empty;
            string filename = string.Empty;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filedir = ofd.FileName.TrimEnd(ofd.FileName.Split('\\').Last().ToCharArray());
                }
                Console.WriteLine(filedir);
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = "c:\\";
                sfd.Filter = "All files (*.*)|*.*";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (sfd.FileName.EndsWith(".txt"))
                        filename = sfd.FileName;
                    else
                        filename = sfd.FileName.Split('.').FirstOrDefault() + ".txt";
                }
                Console.WriteLine(filename);
            }
            var data = GetData(filedir);
            File.WriteAllLines(filename, data);
        }
    }
}
