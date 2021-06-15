using System.IO;
using CC_Library.Predictions;

namespace CC_Library.Datatypes
{
    //Creates a folder in MyDocuments for a specific datatype set
    public static class CreateDataFolder
    {
        public static string CreateFolder(this Datatype dt, WriteToCMDLine wo)
        {
            string folder = dt.ToString().GetMyDocs(wo);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }
    }
}
