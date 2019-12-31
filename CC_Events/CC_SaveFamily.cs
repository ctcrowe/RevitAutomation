using Autodesk.Revit.DB;
using System.IO;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    class SaveFamily
    {
        public static void Main(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                string f = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                string folder = f + "\\CCFamsByID";
                using (Transaction t = new Transaction(doc, "get ID"))
                {
                    t.Start();
                    string id = IDParam.Get(doc);
                    t.Commit();
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    string nf = folder + "\\" + id + ".rfa";
                    string fp = doc.PathName;
                    File.Copy(fp, nf, true);
                }
            }
        }
    }
}
