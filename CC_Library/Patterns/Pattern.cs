using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

namespace CC_Library
{
    public class Pattern
    {
        private List<double[]> Lines;
        private double[] Extents;
        private double scalar;
        public Pattern()
        {
            this.Extents = new double[4]
            { double.MaxValue, double.MaxValue, double.MinValue, double.MinValue};
            this.Lines = new List<double[]>();
        }
        public void AddLine(double[] l)
        {
            this.Lines.Add(l);
            this.Extents[0] = l[0] < l[2] ?
                l[0] < this.Extents[0] ? l[0] : this.Extents[0] :
                l[2] < this.Extents[0] ? l[2] : this.Extents[0];
            this.Extents[1] = l[1] < l[3] ?
                l[1] < this.Extents[1] ? l[1] : this.Extents[1] :
                l[3] < this.Extents[1] ? l[3] : this.Extents[1];
            this.Extents[2] = l[0] > l[2] ?
                l[0] > this.Extents[2] ? l[0] : this.Extents[2] :
                l[2] > this.Extents[2] ? l[2] : this.Extents[2];
            this.Extents[3] = l[1] > l[3] ?
                l[1] > this.Extents[3] ? l[1] : this.Extents[3] :
                l[3] > this.Extents[3] ? l[3] : this.Extents[3];
        }
        private void Reframe()
        {
            var minx = Extents[0];
            var miny = Extents[1];
            var max = Extents[2] > Extents[3] ? Extents[2] : Extents[3];
            this.scalar = max;

            Parallel.For(0, Lines.Count(), i =>
            {
                var x1 = Math.Round((Lines[i][0] - minx) / max, 2);
                var x2 = Math.Round((Lines[i][2] - minx) / max, 2);
                var y1 = Math.Round((Lines[i][1] - miny) / max, 2);
                var y2 = Math.Round((Lines[i][3] - miny) / max, 2);
                Lines[i] = x2 < x1 ? new double[4] { x2, y2, x1, y1 } : new double[4] { x1, y1, x2, y2 };
            });
        }
        public void CreatePattern(WriteToCMDLine write)
        {
            write("Line Count : " + this.Lines.Count());
            write("Extents : " + Extents[0] + "," + Extents[1] + "," + Extents[2] + "," + Extents[3]);
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
                File.WriteAllLines(fp, GetText(fp, write));
            }
        }
        public List<string> GetText(string title, WriteToCMDLine write)
        {
            List<string> lines = new List<string>();
            this.Reframe();
            lines.Add("*" + title.Split('\\').Last().Split('.').First() + ", Scalar is " + scalar);
            lines.Add(";%TYPE=MODEL,");
            for (int n = 0; n < this.Lines.Count(); n++)
            {
                lines.Add(GetLineText(Lines[n]));
            }
            return lines;
        }
        private string GetLineText(double[] line)
        {
            string text= "";
            var Dist = Length(line);
            var AngTo = Angle(line);
            var AngFrom = InvAngle(line);

            bool IsValid = false;
            double DeltaX;
            double DeltaY;
            var Gap = Dist - 1;

            if (Math.Abs(line[0] - line[2]) < 0.001 || Math.Abs(line[3] - line[1]) < 0.001)
            {
                DeltaX = 0;
                DeltaY = 1;
                Gap = Dist - 1;
                IsValid = true;
            }
            else
            {

                var Ang = AngTo < Math.PI ? AngTo : AngFrom;
                var AngZone = Math.Floor(Ang / (Math.PI / 4));
                var XDir = Math.Abs(line[2] - line[0]);
                var YDir = Math.Abs(line[3] - line[1]);
                double Factor = 1;

                switch (AngZone)
                {
                    default:
                    case 0:
                        DeltaY = Math.Abs(Math.Sin(Ang));
                        DeltaX = Math.Abs(Math.Abs(1 / Math.Sin(Ang)) - Math.Abs(Math.Cos(Ang)));
                        break;
                    case 1:
                        DeltaY = Math.Abs(Math.Cos(Ang));
                        DeltaX = Math.Abs(Math.Sin(Ang));
                        break;
                    case 2:
                        DeltaY = Math.Abs(Math.Cos(Ang));
                        DeltaX = Math.Abs(Math.Abs(1 / Math.Cos(Ang)) - Math.Abs(Math.Sin(Ang)));
                        break;
                    case 3:
                        DeltaY = Math.Abs(Math.Sin(Ang));
                        DeltaX = Math.Abs(Math.Cos(Ang));
                        break;
                }
                if (Math.Abs(XDir - YDir) > 0.001)
                {
                    double Ratio = XDir < YDir ? YDir / XDir : XDir / YDir;
                    var RF = Ratio * Factor;
                    var Scaler = XDir < YDir ? 1 / XDir : 1 / YDir;
                    if (Ratio % 1 > 0.001 && 1 - (Ratio % 1) > 0.001)
                    {
                        while (Factor <= 100 && RF % 1 > 0.001 && 1 - RF % 1 > 0.001)
                        {
                            Factor++;
                            RF = Ratio * Factor;
                        }
                        if (Factor > 1 && Factor <= 100)
                        {
                            var AB = XDir * Scaler * Factor;
                            var BC = YDir * Scaler * Factor;
                            var AC = Math.Sqrt((AB * AB) + (BC * BC));
                            double AE = 0;
                            double EF = 1;
                            double x = 1;
                            while (x < AB - 0.5)
                            {
                                var y = x * YDir / XDir;
                                var h = Ang < Math.PI / 2 ? 1 - (y % 1) : y % 1;

                                if (h < EF)
                                {
                                    AE = Math.Sqrt((x * x) + (y * y));
                                    EF = h;
                                }
                                x++;
                            }

                            if (EF < 1)
                            {
                                var EH = BC * EF / AC;
                                var FH = AB * EF / AC;
                                DeltaX = Ang > Math.PI / 2 ? AE - EH : AE + EH;
                                DeltaY = FH;
                                Gap = Dist - AC;
                                IsValid = true;
                            }
                        }
                    }
                    if (Factor == 1)
                    {
                        Gap = Dist - Math.Abs(Factor * 1 / DeltaY);
                        IsValid = true;
                    }
                }
            }
            if (IsValid)
            {
                text += Math.Round(AngTo * 180 / Math.PI, 6) + ",";
                text += line[0] + ",";
                text += line[1] + ",";
                text += Math.Round(DeltaX, 8) + ",";
                text += Math.Round(DeltaY, 8) + ",";
                text += Math.Round(Dist, 8) + ",";
                text += Math.Round(Gap, 8);
            }
            return text;
        }
        private static double Length(double[] Line)
        {
            var x = Line[2] - Line[0];
            var y = Line[3] - Line[1];
            return Math.Sqrt((x * x) + (y * y));
        }
        private static double Angle(double[] line) { return Math.Atan2(line[3] - line[1], line[2] - line[0]); }
        private static double InvAngle(double[] line) { return Math.Atan2(line[1] - line[3], line[0] - line[2]); }
    }
}
