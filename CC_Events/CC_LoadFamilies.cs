using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public class EmbeddedFamilies
    {
        private static string output = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string outfile = output + "//tempfam.rfa";


        public static bool TestID(string id)
        {
            var assembly = typeof(EmbeddedFamilies).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceNames().Any(x => x.Contains(id));
        }

        public static Family run(Document doc, string ID)
        {
            Family fam;
            var assembly = typeof(EmbeddedFamilies).GetTypeInfo().Assembly;
            foreach (string name in assembly.GetManifestResourceNames().Where(x => x.EndsWith(".rfa")))
            {
                if (name.Contains(ID))
                {
                    string outfile = output + "//" + name.Split('.')[name.Split('.').Count() - 2] + ".rfa";
                    string famname = outfile.Split('.').First().Split('\\').Last();
                    using (Stream s = assembly.GetManifestResourceStream(name))
                    using (BinaryReader r = new BinaryReader(s))
                    using (FileStream fs = new FileStream(outfile, FileMode.Create))
                    using (BinaryWriter w = new BinaryWriter(fs))
                    {
                        w.Write(r.ReadBytes((int)s.Length));
                        doc.LoadFamily(outfile, out fam);
                        if (fam != null)
                        {
                            try { fam.Name = name.Split('.')[name.Split('.').Count() - 2]; }
                            catch { }
                            return fam;
                        }
                    }
                }
            }
            if (File.Exists(outfile))
                File.Delete(outfile);
            return null;
        }
    }
}
