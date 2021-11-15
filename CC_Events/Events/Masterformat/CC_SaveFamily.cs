using Autodesk.Revit.DB;
using System.IO;
using Autodesk.Revit.UI;
using System.Linq;

using CC_Library.Predictions;
using CC_Library;

namespace CC_Plugin
{
    internal static class SaveFamilyClass
    {
        public static void SaveFamily(this string fp, string prefix = "test")
        {
            string folder = "CC_Families".GetMyDocs().GetDir();
            

            MasterformatNetwork net = new MasterformatNetwork();
            var div = net.Predict(fp.Split('\\').Last().Split('.').First());
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
                string folder = "CC_Families".GetMyDocs().GetDir();
                string fp = doc.PathName;

                MasterformatNetwork net = new MasterformatNetwork();
                var div = net.Predict(fp.Split('\\').Last().Split('.').First());
                string SubDir = (folder + "\\Division " + div.ToList().IndexOf(div.Max())).GetDir();
                
                string nf = !fp.Split('\\').Last().StartsWith(prefix + "_")?
                    SubDir + "\\" + prefix + "_" + fp.Split('\\').Last().Split('.').First() + ".rfa":
                    SubDir + "\\" + fp.Split('\\').Last().Split('.').First() + ".rfa";
                File.Copy(fp, nf, true);
            }
        }
    }
}
