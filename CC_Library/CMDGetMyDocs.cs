using System;

namespace CC_Library
{
    public static class CMDGetMyDocs
    {
        public delegate void WriteOutput(string s);
        public static string GetMyDocs(this string Subdir, WriteOutput wo)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string subdir = directory +"\\" + Subdir;
            wo(subdir);
            return subdir;
        }
    }
}
