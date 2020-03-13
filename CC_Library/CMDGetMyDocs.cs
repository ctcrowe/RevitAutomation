using System;

namespace CC_Library
{
    public static class CMDGetMyDocs
    {
        public static string GetMyDocs(this string Subdir)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return directory +"\\" + Subdir;
        }
    }
}
