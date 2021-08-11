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
            string f = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string folder = f + "\\CC_Families";
            Dictionary<string, string> fnames = new Dictionary<string, string>();
            List<string> Delete = new List<string>();
            foreach(string dir in Directory.GetDirectories(folder))
            {
                foreach(string file in Directory.GetFiles(dir))
                {
                    
                }
            }
            foreach(strign f in Delete)
            {
                File.Delete(f);
            }
        }
        //foreach(string s in x)
        //create keyvaluepair that is starting and finishing location.
        //if there are overlaps, move one to deletion list (older file)
        //move files to correct location
        //profit.
    }
}
