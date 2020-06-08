using System.Collections.Generic;
using System.Linq;
using System.IO;
using CC_Library;
using Autodesk.Revit.DB;

namespace CC_RevitBasics
{
    public static class CMDCollectKeynotes
    {
        public static void CollectKeynotes(this Document doc)
        {
            string dir = "RECORDED_KEYNOTES";
            string FullDir = dir.GetMyDocs();
            if (!Directory.Exists(FullDir))
                Directory.CreateDirectory(FullDir);

            var path = KeynoteTable.GetKeynoteTable(doc)
                .GetExternalResourceReferences().FirstOrDefault().Value
                .InSessionPath;
            string[] lines = File.ReadAllLines(path);
            List<string> NewLines = new List<string>();
            foreach(string line in lines)
            {
                if(line.Split('\t').Count() > 1)
                {
                    string[] s = line.Split('\t');
                    NewLines.Add(s[1] + ',');
                }
            }
            string FileName = "RECORDED_KEYNOTES\\" + doc.PathName.Split('\\').Last().Split('.').First();
            string filepath = FileName.GetMyDocs() + ".csv";
            File.WriteAllLines(filepath, NewLines);
        }
    }
}