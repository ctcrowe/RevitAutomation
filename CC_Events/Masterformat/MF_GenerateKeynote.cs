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
                        TaskDialog.Show("Keynote Added", number + "\r\n" + Text + "\r\n" + grouping);
                    }
                    else
                    {
                        lines.Add(grouping);
                        lines.Add(Division + ".001\t" + Text + "\t" + grouping);
                        TaskDialog.Show("Keynote Added", Division + "\r\n" + Text + "\r\n" + grouping);
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
            catch(Exception e) { e.OutputError(); }
        }
    }
}
