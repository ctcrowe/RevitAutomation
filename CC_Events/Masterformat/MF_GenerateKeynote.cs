using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;


using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace CC_Plugin
{
    internal static class GenerateKeynote
    {
        internal static string PredictMF(string message)
        {
            string sURL = ("https://us-west2-upheld-now-290121.cloudfunctions.net/PredictMasterformat?message=" + message);

            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (Task<HttpResponseMessage> response = client.GetAsync(sURL))
            using (HttpContent content = response.Result.Content)
            {
                Task<string> result = content.ReadAsStringAsync();
                return result.Result;
            }
        }
        public static void GenKeynote(this Document doc, string Text)
        {
            try
            {
                var Keynotes = KeynoteTable.GetKeynoteTable(doc);
                var exref = Keynotes.GetExternalFileReference();
                var filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(exref.GetAbsolutePath());

                if (!String.IsNullOrEmpty(filename))
                {
                    var lines = File.ReadAllLines(filename).ToList();
                    if (!lines.Where(x => x.Split('\t').Count() > 1).Any(x => x.Split('\t')[1] == Text))
                    {
                        var Division = PredictMF(Text);
                        typeof(MasterformatNetwork).CreateEmbed(Text, Division);
                        var grouping = "Division " + Division;
                        if (lines.Contains(grouping))
                        {
                            var GroupNums = lines.Where(x => x.Split('\t').Count() >= 3).Where(y => y.Split('\t')[2] == "Division " + Division);
                            var max = double.Parse(GroupNums.Max(x => x.Split('\t').First()));
                            var number = max + 0.001;
                            lines.Add(number + "\t" + Text + "\t" + grouping);
                            TaskDialog.Show("Keynote Added", "Key Number : " + number +
                                "\r\nValue : " + Text +
                                "\r\nGroup : " + grouping);
                        }
                        else
                        {
                            lines.Add(grouping);
                            lines.Add(Division + ".001\t" + Text + "\t" + grouping);
                            TaskDialog.Show("Keynote Added", "Key Number : " + Division +
                                ".001\r\nValue : " + Text +
                                "\r\nGroup : " + grouping);
                        }
                        lines.OrderBy(x => x.Split('\t').First());
                        File.WriteAllLines(filename, lines);
                    }
                    using (Transaction t = new Transaction(doc, "Reload Keynote File"))
                    {
                        t.Start();
                        Keynotes.Reload(new KeyBasedTreeEntriesLoadResults());
                        t.Commit();
                    }
                }
                else
                {
                    string fullpath = "";
                    if (doc.IsWorkshared)
                    {
                        var modelpath = ModelPathUtils.ConvertModelPathToUserVisiblePath(doc.GetWorksharingCentralModelPath());
                        filename = modelpath.Split('\\').Last().Split('.').First() + "_Keynotes.txt";
                        fullpath = Directory.GetParent(modelpath).FullName + "\\" + filename;
                    }
                    else
                    {
                        fullpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Keynotes.txt";
                    }
                    if (!File.Exists(fullpath))
                    {
                        using (FileStream fs = File.OpenWrite(fullpath))
                            fs.Close();
                    }
                    using (TransactionGroup tg = new TransactionGroup(doc, "Create file and populate"))
                    {
                        tg.Start();
                        using (Transaction t = new Transaction(doc, "Create Keynote File"))
                        {
                            t.Start();
                            var resource = ExternalResourceReference.CreateLocalResource(
                                doc,
                                ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable,
                                ModelPathUtils.ConvertUserVisiblePathToModelPath(fullpath),
                                PathType.Absolute);
                            Keynotes.LoadFrom(resource, new KeyBasedTreeEntriesLoadResults());
                            t.Commit();
                        }
                        doc.GenKeynote(Text);
                        tg.Commit();
                    }
                }
            }
            catch (Exception e) { e.OutputError(); }
        }
    }
}
