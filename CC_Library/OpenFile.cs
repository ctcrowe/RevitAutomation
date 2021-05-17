using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library
{
    public static class OpenFile
    {
        public static string Run()
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
    }
}
