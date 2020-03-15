using System;
using CC_Library;
using CC_Library.Predictions;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace CreateDataFolder
{
    class Program
    {
        public static void Write(string wo)
        {
            Console.WriteLine(wo);
        }
        static void Main(string[] args)
        {
            DataFile.Masterformat.CreateFolder(new CMDGetMyDocs.WriteOutput(Write));
            DataFile.Subcategory.CreateFolder(new CMDGetMyDocs.WriteOutput(Write));
        }
    }
}
