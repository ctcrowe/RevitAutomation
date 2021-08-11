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
            List<KeyValuePair<string, string>> fnames = new List<KeyValuePair<string, string>>();
            foreach(string dir in Directory.GetDirectories(folder))
            {
                foreach(string file in Directory.GetFiles(dir))
                {
                    
                }
            }
        }
        //foreach(string s in x)
        //create keyvaluepair that is starting and finishing location.
        //move files to correct location
        //profit.
    }
}
