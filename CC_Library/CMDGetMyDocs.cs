using System;
using CC_Library.Predictions;

namespace CC_Library
{
    public static class CMDGetMyDocs
    {
        public static string GetMyDocs(this string Subdir, WriteToCMDLine wo)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string subdir = directory +"\\" + Subdir;
            wo(subdir);
            return subdir;
        }
        public static string GetMyDocs(this string Subdir)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string subdir = directory + "\\" + Subdir;
            return subdir;
        }
    }
}