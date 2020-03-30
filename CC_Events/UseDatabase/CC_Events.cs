using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

namespace CC_Events
{
    #region Event Data
    internal class EventData
    {
        public string FileType;
        public string FileName;
        public string Dir;
        public string User;
        public DateTime Time;
        public string Output;

        private static string PluginLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string LocalManifest = PluginLoc + "\\RIMManifest.txt";
        private static string outmain = File.ReadAllLines(LocalManifest)[1];
        private static string OutDir = outmain + "\\Database\\InputData\\";
        public EventData(string s)
        {
            string mydocs = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.User = System.Environment.UserName;
            this.Dir = mydocs + "\\RevitData\\";
            this.FileType = s;
            this.Time = DateTime.Now;
            this.FileName = Dir + FileType + ".txt";
            this.Output = OutDir + User + '_' + FileType + ".txt";
        }
        public void DirSetup()
        {
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
        }
        public void OutputSetup()
        {
            if (!Directory.Exists(OutDir))
            {
                Directory.CreateDirectory(OutDir);
            }
        }
        public static string GetDataType(string s)
        {
            return s.Split('\\').Last().Split('.').First();
        }
    }
    public class Events
    {
        //This runs the events on startup
        public static void starttime(object sender, DocumentOpenedEventArgs args)
        {
            Document currentDoc = args.Document;
            Autodesk.Revit.ApplicationServices.Application uiApp = currentDoc.Application;
            if (!currentDoc.IsFamilyDocument)
            {
                string fp = currentDoc.PathName;
                BasicFileInfo bfi = BasicFileInfo.Extract(fp);
                string fpath = bfi.CentralPath.Split('\\').Last().Split('.').First();
                string pnum = currentDoc.ProjectInformation.Number.ToString();
                EventData edata = new EventData(pnum);
                edata.DirSetup();

                string[] lines = new string[1];
                string line = edata.User + "," + pnum + "," + fpath + "," + edata.Time + ",Open";
                lines[0] = line;

                File.AppendAllLines(edata.FileName, lines);
            }
        }
        public static void DocClosed(object sender, DocumentClosingEventArgs args)
        {
            Document currentDoc = args.Document;
            Autodesk.Revit.ApplicationServices.Application uiApp = currentDoc.Application;

            if (!currentDoc.IsFamilyDocument)
            {
                EventData ptdata = new EventData("ProjectTime");
                ptdata.DirSetup();
                string fp = currentDoc.PathName;
                BasicFileInfo bfi = BasicFileInfo.Extract(fp);
                string fpath = bfi.CentralPath.Split('\\').Last().Split('.').First();
                string pnum = currentDoc.ProjectInformation.Number.ToString();

                EventData odata = new EventData(pnum);
                string[] Open = File.ReadAllLines(odata.FileName);

                DateTime dtin = DateTime.MinValue;
                DateTime.TryParse(Open.Last().Split(',')[3], out dtin);
                if (dtin != DateTime.MinValue)
                {
                    long timedif = ptdata.Time.Ticks - dtin.Ticks;
                    double hours = TimeSpan.FromTicks(timedif).TotalHours;
                    string[] lines = new string[1];
                    lines[0] = ptdata.User + "," + pnum + "," + fpath + "," + ptdata.Time + "," + hours.ToString() + "," + Open.Last().Split(',')[4];
                    File.AppendAllLines(ptdata.FileName, lines);
                    File.Delete(odata.FileName);
                }
            }
        }
        public static void ViewChangeTime(object sender, ViewActivatedEventArgs args)
        {
            View closingView = args.PreviousActiveView;
            View openedView = args.CurrentActiveView;
            Document closingDoc = closingView.Document;
            Document openedDoc = openedView.Document;
            if (closingView != openedView)
            {
                EventData rdata = new EventData("ProjectTime");
                rdata.DirSetup();
                string fp = closingDoc.PathName;
                BasicFileInfo bfi = BasicFileInfo.Extract(fp);
                string fpath = bfi.CentralPath.Split('\\').Last().Split('.').First();
                string pnum = closingDoc.ProjectInformation.Number.ToString();

                EventData cdata = new EventData(pnum);
                string[] Open = File.ReadAllLines(cdata.FileName);

                DateTime dtin = DateTime.MinValue;
                DateTime.TryParse(Open.Last().Split(',')[3], out dtin);
                if (dtin != DateTime.MinValue)
                {
                    long timedif = rdata.Time.Ticks - dtin.Ticks;
                    double hours = TimeSpan.FromTicks(timedif).TotalHours;
                    string[] clines = new string[1];
                    clines[0] = rdata.User + "," + pnum + "," + fpath + "," + rdata.Time + "," + hours.ToString() + "," + Open.Last().Split(',')[4];
                    File.AppendAllLines(rdata.FileName, clines);
                    File.Delete(cdata.FileName);
                }

                if (!openedDoc.IsFamilyDocument)
                {
                    string ofp = openedDoc.PathName;
                    BasicFileInfo obfi = BasicFileInfo.Extract(fp);
                    string ofpath = obfi.CentralPath.Split('\\').Last().Split('.').First();
                    string opnum = openedDoc.ProjectInformation.Number.ToString();
                    EventData odata = new EventData(opnum);

                    string[] olines = new string[1];
                    olines[0] = odata.User + "," + opnum + "," + ofpath + "," + odata.Time + "," + openedView.Name;
                    File.AppendAllLines(odata.FileName, olines);
                }
            }
        }
        public static void ShutdownData()
        {
            List<string> files = new List<string>();
            EventData edata = new EventData("Shutdown");
            edata.OutputSetup();
            string[] filenames = Directory.GetFiles(edata.Dir);
            foreach (string s in filenames)
            {
                EventData sdata = new EventData(EventData.GetDataType(s));
                string[] lines = File.ReadAllLines(s);
                File.AppendAllLines(sdata.Output, lines);
                files.Add(s);
            }
            foreach (string s in files)
            {
                if (File.Exists(s))
                {
                    File.Delete(s);
                }
            }
        }
    }
    #endregion
}