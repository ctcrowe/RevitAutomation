using System;
using System.Windows.Forms;

namespace CC_Library
{
    public class Command
    {
        public delegate void Cmd(string Entry, string Exit);
        public static void Run(Cmd command)
        {
            string EntryFile = string.Empty;
            string ExitFile = string.Empty;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    EntryFile = ofd.FileName;
                }
                Console.WriteLine(EntryFile);
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = "c:\\";
                sfd.Filter = "All files (*.*)|*.*";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExitFile = sfd.FileName;
                }
                Console.WriteLine(ExitFile);
            }
            command(EntryFile, ExitFile);
        }
    }
}
