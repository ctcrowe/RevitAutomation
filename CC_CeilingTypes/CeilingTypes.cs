using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;

namespace CC_CeilingTypes
{
    public static class CeilingTypes
    {
        private static string output = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string outfile = output + "//tempfam.rfa";


        private static bool TestID(string id)
        {
            var assembly = typeof(CeilingTypes).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceNames().Any(x => x.Contains(id));
        }

        public static void LoadCeilings(this Document doc)
        {
            List<Element> Fams = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToList();
            var assembly = typeof(CeilingTypes).GetTypeInfo().Assembly;
            foreach (string name in assembly.GetManifestResourceNames().Where(x => x.EndsWith(".rfa")))
            {
                string outfile = output + "//" + name.Split('.')[name.Split('.').Count() - 2] + ".rfa";
                string famname = outfile.Split('.').First().Split('\\').Last();
                using (Stream s = assembly.GetManifestResourceStream(name))
                using (BinaryReader r = new BinaryReader(s))
                using (FileStream fs = new FileStream(outfile, FileMode.Create))
                using (BinaryWriter w = new BinaryWriter(fs))
                {
                    w.Write(r.ReadBytes((int)s.Length));
                    doc.LoadFamily(outfile, out Family fam);
                    if (fam != null)
                    {
                        try { fam.Name = name.Split('.')[name.Split('.').Count() - 2]; }
                        catch { }
                    }
                }
            }
            if (File.Exists(outfile))
                File.Delete(outfile);
        }
    }
}
