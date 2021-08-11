using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Plugin.Events
{
    public class FamilyReorginize
    {
        public static void Run()
        {
            string mainfolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string folder = mainfolder + "\\CC_Families";
            Dictionary<string, int> fnames = new Dictionary<string, string>();
            List<string> Delete = new List<string>();
            foreach(string dir in Directory.GetDirectories(folder))
            {
                foreach(string subdir in Directory.GetDirectories(dir))
                {
                    string subname = subdir.Split('\\').Last();
                    foreach(string file in Directory.GetFiles(subdir))
                    {
                        if(file.Split('.').Count() > 2)
                            Delete.Add(file);
                        else
                        {
                            if(fnames.Keys.Any(x => x.split('\\').Last() == file.Split('\\').Last()))
                            {
                                //check date modified
                                //compare to fnames.keys
                                //replace as required
                                if(...)
                                {
                                    Delete.Add(fnames.Keys.Where(x => x.Split('\\').Last() == file.Split('\\').Last()).First());
                                }
                                else
                                    Delete.Add(file);
                            }
                            else
                            {
                                fnames.Add(file, Datatype.Masterformat.PredictSingle(file.Split('\\').Last().Split('.').First()));
                            }
                        }
                    }
                }
                foreach(string file in Directory.GetFiles(dir))
                {
                    Delete.Add(file);
                }
            }
            foreach(string f in Delete)
            {
                File.Delete(f);
            }
        }
    }
}
