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
    public class BrickPattern
    {
        private double Width { get; set; }
        private double Height { get; set; }
        private double GroutX { get; set; }
        private double GroutY { get; set; }
        private int Ratio { get; set; }
        public BrickPattern(double W, double H, double Gx = 0, double Gy = 0, int TR = 2)
        {
            this.Width = Math.Abs(W);
            this.Height = Math.Abs(H);
            this.GroutX = Math.Abs(Gx);
            this.GroutY = Math.Abs(Gy);
            this.Ratio = TR == 0 ? 1 : Math.Abs(TR);
        }
        public void SetWidth(double W) { this.Width = Math.Abs(W); }
        public void SetHeight(double H) { this.Height = Math.Abs(H); }
        public void SetGrout(double G) { this.Grout = Math.Abs(G); }
        public void SetRatio(int TR) { this.Ratio = TR == 0 ? 1 : Math.Abs(TR); }
        public void WritePattern(Document doc, string fn)
        {
            double XOffset = 1.0 - (1.0 / Ratio);
            List<string> lines = new List<string>();
            List<string> grids = new List<string>();
            lines.Add("*" + fn.Split('\\').Last().Split('.').First());
            lines.Add(";%TYPE=MODEL,");
            grids.Add("0,0,0," + (XOffset * (Width + GroutX)) + "," + (Height + GroutY) + "," + Width + "," + (-1 * GroutX));
            grids.Add("90,0,0," + (Height + GroutY) + "," + ((Width + GroutX) / Ratio) + "," + Height + "," + (-1 * (((Ratio - 1) * Height) + (Ratio * GroutY))));
            if(GroutX > 0)
                grids.Add("0,0," + Height + "," + (XOffset * (Width + GroutX)) + "," + (Height + GroutY) + "," + Width + "," + (-1 * GroutX));
            if (GroutY > 0)
                grids.Add("90," + Width + ",0," + (Height + GroutY) + "," + ((Width + GroutX) / Ratio) + "," + Height + "," + (-1 * (((Ratio - 1) * Height) + (Ratio * GroutY))));
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
                BrickPattern p = new BrickPattern(W, H, G, G, ratio);
                p.WritePattern(doc, fp);
            }
        }
        public static void CreatePattern(Document doc, double W, double H, double Gx, double Gy, int ratio = 2)
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
                BrickPattern p = new BrickPattern(W, H, Gx, Gy, ratio);
                p.WritePattern(doc, fp);
            }
        }
    }
}
