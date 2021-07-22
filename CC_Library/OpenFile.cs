using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library
{
    public static class Files
    {
        public static string Open()
        {
            string filepath = null;
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a file",
                Title = "Open a file"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filepath = ofd.FileName;
            }
            return filepath;
        }
        public static string GetFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog1.FileName;
            }
            return null;
        }
    }
}
