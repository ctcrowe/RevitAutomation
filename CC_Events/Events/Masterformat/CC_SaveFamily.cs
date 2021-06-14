using Autodesk.Revit.DB;
using System.IO;
using Autodesk.Revit.UI;
using System.Linq;

using CC_Library.Predictions;

namespace CC_Plugin
{
    class SaveFamily
    {
        public static void Main(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                string ftype = "";
                switch(doc.OwnerFamily.FamilyCategoryId.IntegerValue)
                {
                    default:
                        ftype = "Element";
                        break;
                    case (int)BuiltInCategory.OST_DetailComponents:
                        ftype = "Detail";
                        break;
                    case (int)BuiltInCategory.OST_ProfileFamilies:
                        ftype = "Profile";
                        break;
                }
                string f = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                string folder = f + "\\CC_Families";
                using (Transaction t = new Transaction(doc, "get ID"))
                {
                    string fp = doc.PathName;
                    string Division = "Division " + MasterformatNetwork.Predict(fp.Split('\\').Last().Split('.').First());
                    string SubDir = folder + "\\" + Division;
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    if (!Directory.Exists(SubDir))
                        Directory.CreateDirectory(SubDir);
                    string SubSubDir = SubDir + "\\" + ftype;
                    if (!Directory.Exists(SubSubDir))
                        Directory.CreateDirectory(SubSubDir);
                    string nf = SubSubDir + "\\" + fp.Split('\\').Last().Split('.').First() + ".rfa";
                    File.Copy(fp, nf, true);
                }
            }
        }
    }
}