using System.Reflection;
using System.IO;

namespace CC_Plugin
{
    public static class ResetParamLibrary
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static void Run()
        {
            if (File.Exists(SharedParams))
            {
                File.Delete(SharedParams);
            }
            using (FileStream stream = File.Create(SharedParams))
            {
                stream.Close();
            }
        }
    }
}
