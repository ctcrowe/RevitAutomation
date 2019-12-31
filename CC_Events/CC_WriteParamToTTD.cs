using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Revit.DB;

namespace CC_Plugin
{
    internal class WriteParamToTTD
    {
        private static string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string subdir = dir + "\\CC_Data";

        public static void Run(Document doc, string Dataset, List<Param> paramset)
        {
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);

            string data = DateTime.Now.ToString("yyyyMMddhhmmss");
            string file = "";
            if (!doc.IsFamilyDocument)
            {
                file = subdir + "\\" + doc.ProjectInformation.Name + "_" + Dataset + ".txt";
                for (int i = 0; i < paramset.Count(); i++)
                {
                    data += "\t" + doc.ProjectInformation.get_Parameter(paramset[i].ID).AsValueString();
                }
            }
            else
            {
                file = subdir + "\\" + doc.OwnerFamily.Name + "_" + Dataset + ".txt";
                for(int i = 0; i < paramset.Count(); i++)
                {
                    FamilyParameter par = doc.FamilyManager.get_Parameter(paramset[i].ID);
                    data += "\t" + doc.FamilyManager.CurrentType.AsString(par);
                }
            }
            File.AppendAllLines(file, new List<string>() { data });
        }
    }
}