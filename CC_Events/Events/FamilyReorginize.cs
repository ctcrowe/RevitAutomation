using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace CC_Plugin
{
    public class FamilyReorganize
    {
        public static void Run()
        {
            string mainfolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string folder = mainfolder + "\\CC_Families";
            Hashtable fnames = new Hashtable();
            List<string> allfiles = new List<string>();
            foreach(string dir in Directory.GetDirectories(folder))
            {
                foreach(string file in Directory.GetFiles(dir))
                {
                    allfiles.Add(file);
                    var key = file.Split('\\').Last().Split('.').First();
                    fnames[key] = fnames[key] == null?
                        file, DateTime.Compare(File.GetLastWriteTime(fnames[key]), File.GetLastWriteTime(file)) < 0?
                        file: fnames[key];
                }
            }
            foreach(string f in Delete)
            {
                File.Delete(f);
            }
            foreach(var f in allfiles)
            {
                if(!fnames.Values.Contains(f))
                    File.Delete(f);
                else
                {
                    string val = f.Split('\\').Last().Split('.').First();
                    int numb = Datatype.Masterformat.PredictSingle(val);
                    string subfolder = folder + "\\Division " + numb;
                    if(!Directory.Exists(subfolder))
                        Directory.CreateDirectory(subfolder);
                    File.Move(fnames[val], subfolder + "\\" + val + ".rfa");
                }
            }
        }
    }
}
