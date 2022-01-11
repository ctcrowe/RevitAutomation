using Autodesk.Revit.DB;
using System.IO;
using Autodesk.Revit.UI;
using System.Linq;

using CC_Library.Predictions;
using CC_Library;

namespace CC_Plugin
{
    internal static class MFSaveFamily
    {
        public static void SaveFamily(this string fp, string prefix = "test")
        {
            string folder = "CC_Families".GetMyDocs().GetDir();
            
            var div = MasterformatNetwork.Predict(fp.Split('\\').Last().Split('.').First(), new WriteToCMDLine(CMDLibrary.WriteNull));
            string Division = "Division " + div.ToList().IndexOf(div.Max());
            string SubDir = (folder + "\\Division " + div.ToList().IndexOf(div.Max())).GetDir();
                
            string nf = !fp.Split('\\').Last().StartsWith(prefix + "_")?
                SubDir + "\\" + prefix + "_" + fp.Split('\\').Last().Split('.').First() + ".rfa":
                SubDir + "\\" + fp.Split('\\').Last().Split('.').First() + ".rfa";
            File.Copy(fp, nf, true);
        }
        public static void Main(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                string prefix = "";
                switch(doc.OwnerFamily.FamilyCategoryId.IntegerValue)
                {
                    default:
                        prefix = "Ele";
                        break;
                    case (int)BuiltInCategory.OST_DetailComponents:
                        prefix = "Det";
                        break;
                    case (int)BuiltInCategory.OST_ProfileFamilies:
                        prefix = "Pro";
                        break;
                }
                string fp = doc.PathName;
                fp.SaveFamily(prefix);
            }
        }
    }
}