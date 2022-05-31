using Autodesk.Revit.DB;
using System.IO;
using System;
using System.Linq;

using CC_Plugin.Parameters;

using CC_Library.Parameters;
using CC_Library.Predictions;
using CC_Library;

namespace CC_Plugin
{
    internal static class MFSaveFamily
    {
        public static void SetFamily(string fn, Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                var output = MasterformatNetwork.Predict(fn, CMDLibrary.WriteNull);
                var Masterformat = output.ToList().IndexOf(output.Max());
                using (TransactionGroup tg = new TransactionGroup(doc, "Set MF"))
                {
                    tg.Start();
                    using (Transaction t = new Transaction(doc, "Add MF Param"))
                    {
                        t.Start();
                        try { doc.AddParam(Params.Masterformat); }
                        catch (Exception e) { e.OutputError(); }
                        t.Commit();
                    }
                    if (doc.FamilyManager.Types.Size < 1)
                    {
                        using (Transaction t = new Transaction(doc, "Create Family Type"))
                        {
                            t.Start();
                            doc.FamilyManager.NewType("Automatic Type");
                            t.Commit();
                        }
                    }
                    using (Transaction t = new Transaction(doc, "Set MF Param"))
                    {
                        t.Start();
                        try { doc.SetFamilyParam(Params.Masterformat, Masterformat.ToString()); }
                        catch (Exception e) { e.OutputError(); }
                        t.Commit();
                    }
                    tg.Commit();
                }
            }
        }
        public static void SaveFamily(string fn, Document doc)
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
            
                var div = MasterformatNetwork.Predict(fp.Split('\\').Last().Split('.').First(), new WriteToCMDLine(CMDLibrary.WriteNull));
                typeof(MasterformatNetwork).CreateEmbed(fp.Split('\\').Last().Split('.').First(), div.ToString());
                string Division = "Division " + div.ToList().IndexOf(div.Max());
                string SubDir = (folder + "\\Division " + div.ToList().IndexOf(div.Max())).GetDir();
                
                string nf = !fp.Split('\\').Last().StartsWith(prefix + "_")?
                SubDir + "\\" + prefix + "_" + fp.Split('\\').Last().Split('.').First() + ".rfa":
                SubDir + "\\" + fp.Split('\\').Last().Split('.').First() + ".rfa";
                File.Copy(fp, nf, true);
            }
        }
    }
}
