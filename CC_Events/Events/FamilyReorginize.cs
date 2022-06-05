using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using CC_Library;
using CC_Library.Predictions;

namespace CC_Plugin
{
    public class FamilyReorganize
    {
        public static void Run()
        {
            try
            {
                string mainfolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                string folder = mainfolder + "\\CC_Families";
                Hashtable fnames = new Hashtable();
                List<string> allfiles = new List<string>();
                foreach (string dir in Directory.GetDirectories(folder))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        allfiles.Add(file);
                        var key = file.Split('\\').Last().Split('.').First();
                        fnames[key] = fnames[key] == null ?
                            file : DateTime.Compare(File.GetLastWriteTime((string)fnames[key]), File.GetLastWriteTime(file)) < 0 ?
                            file : fnames[key];
                    }
                }
                foreach (var f in allfiles)
                {
                    if (!fnames.ContainsValue(f))
                        File.Delete(f);
                    else
                    {
                        string val = f.Split('\\').Last().Split('.').First();
                        var vals = MasterformatNetwork.Predict(val, CMDLibrary.WriteNull);
                        int numb = vals.ToList().IndexOf(vals.Max());
                        string subfolder = folder + "\\Division " + numb;
                        if (!Directory.Exists(subfolder))
                            Directory.CreateDirectory(subfolder);
                        File.Move((string)fnames[val], subfolder + "\\" + val + ".rfa");
                    }
                }
            }
            catch (Exception e) { e.OutputError(); }
        }
    }
}
