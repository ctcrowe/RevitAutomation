using System.Reflection;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public static class CommandLibrary
    {
        public delegate void DocCommand(Document doc)
        public static void Transact(DocCommand dc, Document doc)
        {
            using(Transaction t = new Transaction(doc, "Run Command"))
            {
                t.start();
                dc(doc);
                t.Commit();
            }
        }
    }
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
