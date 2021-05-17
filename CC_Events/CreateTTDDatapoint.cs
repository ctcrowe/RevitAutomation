using System;
using System.IO;
using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace CC_Plugin
{
    internal static class CreateDataPoint
    {
        private static string Write(string s)
        {
            return null;
        }
        public static void CreateTTDData(this string obj, string id, string value, Datatype dt)
        {
            string[] lines = new string[4] { DateTime.Now.ToString("yyyyMMddhhmmss"), obj, id, value };
            string folder = dt.CreateFolder(new WriteToCMDLine(Write));
            string file = folder + "\\" + Guid.NewGuid().ToString("N") + ".txt";
            File.WriteAllLines(file, lines);
        }
    }
}