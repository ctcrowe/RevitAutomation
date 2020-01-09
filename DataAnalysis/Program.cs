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
            Mastersection.Main();
            /*
            string filedir = string.Empty;
            string filename = string.Empty;

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
                    if (sfd.FileName.Contains("."))
                        filename = sfd.FileName.Split('.').FirstOrDefault();
                    else
                        filename = sfd.FileName;
                }
                Console.WriteLine(filename);
            }
            Datasort.Qualify(filedir, filename, 365);
            */
        }
    }
}
