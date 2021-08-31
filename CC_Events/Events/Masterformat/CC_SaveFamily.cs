using Autodesk.Revit.DB;
using System.IO;
using Autodesk.Revit.UI;
using System.Linq;

using CC_Library.Predictions;

namespace CC_Plugin
{
    internal static class SaveFamilyClass
    {
        public static void SaveFamily(this string fp, string prefix = "test")
        {
            string f = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string folder = f + "\\CC_Families";
            
            Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
            s.TextInput = fp.Split('\\').Last().Split('.').First();

            MasterformatNetwork net = new MasterformatNetwork();
            var div = net.Predict(s);
            string Division = "Division " + div.ToList().IndexOf(div.Max());
            string SubDir = folder + "\\" + Division;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (!Directory.Exists(SubDir))
                Directory.CreateDirectory(SubDir);
            if(!fp.Split('\\').Last().StartsWith(prefix + "_"))
            {
                string nf = SubDir + "\\" + prefix + "_" + fp.Split('\\').Last().Split('.').First() + ".rfa";
                File.Copy(fp, nf, true);
            }
            else
            {
                string nf = SubDir + "\\" + fp.Split('\\').Last().Split('.').First() + ".rfa";
                File.Copy(fp, nf, true);
             }
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
                string f = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                string folder = f + "\\CC_Families";
                string fp = doc.PathName;
                Sample s = new Sample(CC_Library.Datatypes.Datatype.Masterformat);
                s.TextInput = fp.Split('\\').Last().Split('.').First();

                MasterformatNetwork net = new MasterformatNetwork();
                var div = net.Predict(s);
                string Division = "Division " + div.ToList().IndexOf(div.Max());
                string SubDir = folder + "\\" + Division;
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                if (!Directory.Exists(SubDir))
                    Directory.CreateDirectory(SubDir);
                
                string nf = !fp.Split('\\').Last().StartsWith(prefix + "_")?
                    Subdir + "\\" + prefix + "_" + fp.Split('\\').Last().Split('.').First() + ".rfa":
                    SubDir + "\\" + fp.Split('\\').Last().Split('.').First() + ".rfa";
                File.Copy(fp, nf, true);
            }
        }
    }
}
