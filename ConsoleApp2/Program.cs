using System;
using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;

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
            Datatype.Masterformat.CreateFolder(new WriteToCMDLine(Write));
            Datatype.Subcategory.CreateFolder(new WriteToCMDLine(Write));
        }
    }
}
