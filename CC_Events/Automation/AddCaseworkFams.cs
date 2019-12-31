using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Reflection;

namespace CC_Plugin
{
    public class CaseworkFams
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";
        private static Param CheckParam = ParameterLibrary.FamilyCheck;

        public static void Execute(Document doc)
        {
            ResetParamLibrary.Run();

            Application App = doc.Application;
            App.SharedParametersFilename = SharedParams;
            DefinitionFile deffile = App.OpenSharedParameterFile();


            if (doc.IsFamilyDocument)
            {
                {
                    using (TransactionGroup tg = new TransactionGroup(doc, "Add Casework Fams"))
                    {
                        tg.Start();
                        EmbeddedFamilies.run(doc, "Cab");

                        using (Transaction t = new Transaction(doc, "Params"))
                        {
                            try
                            {
                                Definition def = AddParam.SetupParam(deffile, doc, CheckParam);
                                t.Start();
                                AddParam.Run(def, doc, CheckParam);
                                t.Commit();
                            }
                            catch
                            {
                            }
                        }
                        tg.Commit();
                    }
                }
            }
        }
    }
}