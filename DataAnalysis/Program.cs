using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Linq;


namespace DataAnalysis
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string filedir = string.Empty;
            string filename = string.Empty;
            Datasort.DataQualifier dq = new Datasort.DataQualifier(DataCheck);

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filedir = ofd.FileName.TrimEnd(ofd.FileName.Split('\\').Last().ToCharArray());
                }
                Console.WriteLine(filedir);
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = "c:\\";
                sfd.Filter = "All files (*.*)|*.*";
                sfd.RestoreDirectory = true;

                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    if (sfd.FileName.EndsWith(".txt"))
                        filename = sfd.FileName;
                    else
                        filename = sfd.FileName.Split('.').FirstOrDefault() + ".txt";
                }
                Console.WriteLine(filename);
            }
            Datasort.Count(filedir, filename, 365, dq);
        }
        public static bool DataCheck(XDocument xdoc)
        {
            return true;
        }
    }
}