using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;

namespace CC_Plugin
{
    public static class FamilyUnpacker
    {
        private static string output = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static void UnpackFamilies(this Document doc, Assembly assembly)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Load Families"))
            {
                tg.Start();
                foreach (string name in assembly.GetManifestResourceNames().Where(x => x.EndsWith(".rfs")))
                {
                    var fam = doc.UnpackFamily(assembly, name);
                }
                tg.Commit();
            }
        }
        public static Family UnpackFamily(this Document doc, Assembly assembly, string name)
        {
            using (TransactionGroup tg = new TransactionGroup(doc, "Load Families"))
            {
                Family fam;
                using (Transaction t = new Transaction(doc, "Load " + name))
                {
                    t.Start();
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
                        }
                    }
                    if (File.Exists(outfile))
                        File.Delete(outfile);
                    t.Commit();
                }
                return fam;
            }
        }
    }
}
