using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace CC_Patterns
{
    /// <summary>
    /// a brick pattern is defined by 3 variables
    ///     Width
    ///     Height
    ///     Grout Space - Grout can be 0
    ///  
    /// The pattern can be formulaically determined with 4 lines.
    /// line 1 : angle = 0, X = 0, Y = 0, ShiftX = (Width + Grout) / 2, ShiftY = Height + Grout, PD = Width, PU = -Grout
    /// line 2 : angle = 0, X = 0, Y = Height, ShiftX = (Width + Grout) / 2, ShiftY = Height + Grout, PD = Width, PU = -Grout
    /// line 3 : angle = 90, X = 0, Y = 0, ShiftX = Height + Grout, ShiftY = (Width + Grout) / 2, PD = Height, PU = Height + 2 * Grout
    /// line 4 : angle = 90, X = Width, Y = 0, ShiftX = Height + Grout, ShiftY = (Width + Grout) / 2, PD = Height, PU = Height + 2 * Grout
    /// </summary>
    public class PlankPattern
    {
        private double Width { get; set; }
        private double Height { get; set; }
        private double Grout { get; set; }
        public PlankPattern(double W, double H, double G = 0)
        {
            this.Width = Math.Abs(W);
            this.Height = Math.Abs(H);
            this.Grout = Math.Abs(G);
        }
        public void SetWidth(double W) { this.Width = Math.Abs(W); }
        public void SetHeight(double H) { this.Height = Math.Abs(H); }
        public void SetGrout(double G) { this.Grout = Math.Abs(G); }
        public void WritePattern(Document doc, string fn)
        {
            Random r = new Random();
            List<string> lines = new List<string>();
            List<string> grids = new List<string>();
            lines.Add("*" + fn.Split('\\').Last().Split('.').First());
            lines.Add(";%TYPE=MODEL,");
            
            for(int i = 0; i < 10; i++)
            {
                var rand = r.NextDouble();
                grids.Add("0," + (rand * Width) + "," + (i * (Height + Grout)) + "," + (Width + Grout) + "," + (10 * (Grout + Height)) + "," + Width + "," + (-1 * Grout);
                grids.Add("0," + (rand * Width) + "," + (Height + (i * (Height + Grout))) + "," + (Width + Grout) + "," + (10 * (Grout + Height)) + "," + Width + "," + (-1 * Grout);
                grids.Add("90," + (rand * Width) + "," + (i * (Height + Grout)) + "," + (10 * (Grout + Height)) + "," + (Width + Grout) + "," + Height + "," + (-1 * (10 * (Grout + Height)));
                grids.Add("90," + (Width + (rand * Width)) + "," + (i * (Height + Grout)) + "," + (10 * (Grout + Height)) + "," + (Width + Grout) + "," + Height + "," + (-1 * (10 * (Grout + Height)));
            }
                          
            lines.AddRange(grids);
            File.WriteAllLines(fn, lines);

            doc.ImportPattern(fn.Split('\\').Last().Split('.').First(), grids);
        }
        public static void CreatePattern(Document doc, double W, double H, double G = 0, int ratio = 2)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = "Create a pattern file",
                Filter = "PAT files (*.pat)|*.pat",
                Title = "Create a pat file"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var fp = sfd.FileName;
                if (fp.EndsWith(".txt"))
                    fp.Replace(".txt", ".pat");
                if (!fp.EndsWith(".pat"))
                    fp += ".pat";
                BrickPattern p = new BrickPattern(W, H, G, ratio);
                p.WritePattern(doc, fp);
            }
        }
    }
}
