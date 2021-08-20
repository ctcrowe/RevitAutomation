using System;
using System.Collections.Generic;
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
            Dictionary<string, string> fnames = new Dictionary<string, string>();
            List<string> Delete = new List<string>();
            foreach(string dir in Directory.GetDirectories(folder))
            {
                foreach(string file in Directory.GetFiles(dir))
                {
                    var key = file.Split('\\').Last().Split('.').First();
                    if(fnames.ContainsKey(key))
                    {
                        var orig = fnames[key];
                        if(DateTime.Compare(File.GetLastWriteTime(orig), File.GetLastWriteTime(file)) < 0)
                        {
                            Delete.Add(fnames[key]);
                            fnames[key] = file;
                        }
                        else
                            Delete.Add(file);
                    }
                    else
                        fnames.Add(file.Split('\\').Last().Split('.').First(), file);
                }
            }
            foreach(string f in Delete)
            {
                File.Delete(f);
            }
            foreach(var f in fnames)
            {
                int numb = Datatype.Masterformat.PredictSingle(f.Key);
                string subfolder = folder + "\\Division " + numb;
                if(!Directory.Exists(subfolder))
                   Directory.CreateDirectory(subfolder);
                File.Move(f.Value, subfolder + "\\" + f.Key + ".rfa");
            }
        }
    }
}
