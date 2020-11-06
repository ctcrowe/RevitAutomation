using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library;
using System.Xml.Linq;
using System.IO;

namespace CC_Library.Materials
{
    public class CCMaterial
    {
        public string Category { get; }
        public string Model { get; }
        public string Manufacturer { get; set; }
        public string Color { get; set;  }
        public List<string> Patterns { get; set; }
        public List<string> Projects { get; set; }
        public string Description { get; set; }

        public CCMaterial(string cat, string mdl)
        {
            this.Category = cat;
            this.Model = mdl;
            this.Patterns = new List<string>();
            this.Projects = new List<string>();
        }

        public void AddProject(string prj)
        {
            if (!this.Projects.Contains(prj))
                this.Projects.Add(prj);
        }

        public void AddPattern(string pat)
        {
            if (!this.Patterns.Contains(pat))
                this.Patterns.Add(pat);
        }

        public void Save()
        {
            var dir = "Materials".GetMyDocs();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string folder = dir + "\\" + this.Category;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            XDocument doc = new XDocument(new XElement(this.Category.ToString()))
            { Declaration = new XDeclaration("1.0", "utf-8", "yes") };

        
            doc.Root.Add(new XAttribute("Model", this.Model));
            doc.Root.Add(new XAttribute("Manufacturer", this.Manufacturer));
            doc.Root.Add(new XAttribute("Color", this.Color));
            doc.Root.Add(new XAttribute("Description", this.Description));
            
            if (Patterns.Any())
            {
                foreach (var pat in this.Patterns)
                {
                    XElement ele = new XElement("Pattern");
                    ele.Add(new XAttribute("Description", pat));
                    doc.Root.Add(ele);
                }
            }
            if (Projects.Any())
            {
                foreach (var prj in this.Projects)
                {
                    XElement ele = new XElement("Project");
                    ele.Add(new XAttribute("Description", prj));
                    doc.Root.Add(ele);
                }
            }

            string fn = folder + "\\" + this.Model + ".xml";
            doc.Save(fn);
        }

        public static CCMaterial Get(string cat, string model)
        {
            var dir = "Materials".GetMyDocs();
            if (Directory.Exists(dir))
            {
                string folder = dir + "\\" + cat;
                if (Directory.Exists(folder))
                {
                    var files = Directory.GetFiles(folder);
                    if (files.Any(x => x == folder + "\\" + model))
                        return Load(files.Where(x => x.Split('\\').Last().Split('.').First() == model).First());
                }
            }
            return new CCMaterial(cat, model);
        }

        private static CCMaterial Load(string filename)
        {
            var doc = XDocument.Load(filename);

            CCMaterial mat = new CCMaterial(doc.Root.Name.LocalName, doc.Root.Attribute("Model").Value);
            mat.Description = doc.Root.Attribute("Description").Value;
            mat.Manufacturer = doc.Root.Attribute("Manufacturer").Value;
            mat.Color = doc.Root.Attribute("Color").Value;

            var Patterns = doc.Root.Elements("Pattern");
            var Projects = doc.Root.Elements("Project");
            for(int i = 0; i < Patterns.Count(); i++)
            {
                mat.Patterns.Add(Patterns.ElementAt(i).Attribute("Description").Value);
            }
            for(int i = 0; i < Projects.Count(); i++)
            {
                mat.Projects.Add(Projects.ElementAt(i).Attribute("Description").Value);
            }

            return mat;
        }
    }
}
