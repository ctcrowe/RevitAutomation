using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace CC_Library
{
    public class GenPredictions
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CC_XMLData";
        private static string FileName = directory + "\\CC_MFData.xml";
        
        public static void Run()
        {
            if(Directory.Exists(Dataset))
            {
                string[] files = Directory.GetFiles(folder);
                foreach(string f in files)
                {
                    XDocument doc = XDocument.Load(f);
                    if(doc.Root.Attribute("MFSection") != null)
                    {
                        
                    }
                }
            }
        }
    }
}
