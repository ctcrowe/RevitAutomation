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
using CC_Library.Parameters;

using CC_Plugin.Parameters;

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
            var Keynotes = KeynoteTable.GetKeynoteTable(doc);
            var exref = Keynotes.GetExternalFileReference();
            var filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(exref.GetAbsolutePath());
            
            var lines = File.ReadAllLines(filename).ToList();
            if (!lines.Any(x => x.Split('\t')[1] == Text))
            {
                var Division = PredictMF(Text);
                typeof(MasterformatNetwork).CreateEmbed(Text, Division);
                var grouping = "Division " + Division;
                if (lines.Contains(grouping))
                {
                    var maxoptions = lines.Where(x => int.Parse(x.Split('\t').First().Substring(0, 2)) == int.Parse(Division));
                    var max = double.Parse(maxoptions.Max());
                    var number = max + 0.001;
                    lines.Add(number + "\t" + Text + "\t" + grouping);
                }
                else
                {
                    lines.Add(grouping);
                    lines.Add(Division + ".001\t" + Text + "\t" + grouping);
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
    }
}
