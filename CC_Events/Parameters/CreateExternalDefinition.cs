using System;
using System.Collections.Generic;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

using CC_Library.Parameters;
using CC_Library;


namespace CC_Plugin.Parameters
{
    internal static class CreateExternalDefinition
    {
        private const string Group = "Automatic";
        public static Definition CreateDefinition(this Param p, Document doc)
        {
            string localname = "CC_SharedParams.txt";
            string FullName = localname.GetMyDocs();
            if (File.Exists(FullName))
                File.Delete(FullName);

            using (StreamWriter stream = new StreamWriter(FullName))
            {
                stream.Close();
            }

            Application app = doc.Application;
            app.SharedParametersFilename = FullName;
            DefinitionFile df = app.OpenSharedParameterFile();

            if (df.Groups.get_Item(Group) == null)
            {
                DefinitionGroup newgroup = df.Groups.Create(Group);
                if (df.Groups.get_Item(Group).Definitions.get_Item(p.Name) == null)
                    return newgroup.Definitions.Create(p.CreateOptions());
                else
                    return newgroup.Definitions.get_Item(p.Name);
            }
            DefinitionGroup group = df.Groups.get_Item(Group);
            if (df.Groups.get_Item(Group).Definitions.get_Item(p.Name) == null)
                return group.Definitions.Create(p.CreateOptions());
            return group.Definitions.get_Item(p.Name);
        }
    }
}
