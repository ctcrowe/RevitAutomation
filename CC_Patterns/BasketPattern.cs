using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace CC_Patterns
{
    /// <summary>
    /// a Herringbone pattern is defined by 2 variables - This could be expanded to 3 to add a grout line
    ///     Width
    ///     Height
    ///     Grout (TO BE ADDED) - Grout can be 0
    ///  NOTE: a square herrinbone pattern would be strange but could exist.
    /// The pattern can be formulaically determined with 2 lines (4 when grout is added)
    /// line 1 : angle = 0, X = 0, Y = 0, ShiftX = Width + Grout, ShiftY = Width + Grout, PD = Width + Height, -1 * (Math.Abs(Width - Height) + Grout)
    /// line 2 : angle = 0, X = 0, Y = Height, ShiftX = Width + Grout, ShiftY = Width + Grout, PD = Width + Height, -1 * (Math.Abs(Width - Height) + Grout)
    /// line 3 : angle = 90, X = Width, Y = 0, ShiftX = Width + Grout, ShiftY = -1 * (Width + Grout), PD = Width + Height, PU = -1 * Math.Abs(Width - Height)
    /// </summary>
    public class BasketPattern
    {
        private double Width { get; set; }
        private double Height { get; set; }
        private double Grout { get; set; }
        public BasketPattern(double W, double H)
        {
            this.Width = Math.Abs(W);
            this.Height = Math.Abs(H);
        }
        public void SetWidth(double W) { this.Width = Math.Abs(W); }
        public void SetHeight(double H) { this.Height = Math.Abs(H); }
        public void WritePattern(string fn)
        {
            var floor = Width > Height ? Math.Floor(Width / Height) : Math.Floor(Height / Width);
            var R = Width > Height ? Width % Height : Height % Width;
            Grout = Width == Height ? 0 : R / (floor - 1);
            var offset = Width + Grout;
            var h2 = Height + Grout;

            List<string> lines = new List<string>();
            lines.Add(";%UNITS=INCHES");
            lines.Add("*" + fn.Split('\\').Last().Split('.').First() + ",Width = " + Width + " Height = " + Height + " Grout Spacing = " + Grout);
            lines.Add(";%TYPE=MODEL,");

            lines.Add("0,0,0" + offset + "," + offset + "," + Height + "," + -offset);
            lines.Add("0," + h2 + ",0," + offset + "," + offset + "," + Height + "," + -offset);
            lines.Add("0," + (2 * h2) + ",0," + offset + "," + offset + "," + Height + "," + -offset);
            lines.Add("90,0,0," + offset + "," + offset + "," + Width + "," + -offset);
            lines.Add("90," + Height + ",0," + offset + "," + offset + "," + Width + "," + -offset);
            lines.Add("90," + h2 + ",0," + offset + "," + offset + "," + Width + "," + -offset);
            lines.Add("90," + (h2 + Height) + ",0," + offset + "," + offset + "," + Width + "," + -offset);
            lines.Add("90," + (2 * h2) + ",0," + offset + "," + offset + "," + Width + "," + -offset);
            lines.Add("90," + ((2 * h2) + Height) + ",0," + offset + "," + offset + "," + Width + "," + -offset);

            File.WriteAllLines(fn, lines);
        }
        public static string CreatePattern(Document doc, double W, double H)
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
                HerringbonePattern p = new HerringbonePattern(W, H);
                p.WritePattern(doc, fp);
                return fp;
            }
            return null;
        }
    }
}
