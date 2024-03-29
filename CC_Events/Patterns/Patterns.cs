﻿using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

using CC_Library;
using CC_Library.Datatypes;

namespace CC_Plugin
{
    //Reframe the hatch from 0 to 1, include a comment that tells the user what to scale it to!!!!
    public class HatchEditor
    {
        private static string write(string s)
        {
            TaskDialog.Show("Output", s);
            return s;
        }
        public static void EditHatch(Document doc)
        {
            var v = doc.ActiveView;
            var lines = new FilteredElementCollector(doc, v.Id).OfCategory(BuiltInCategory.OST_Lines).ToElementIds().ToList();

            DrawnPattern p = new DrawnPattern();
            for(int i = 0; i < lines.Count(); i++)
            {
                var line = doc.GetElement(lines[i]) as DetailLine;
                if (line != null)
                {
                    p.AddLine(new double[4]
                    {
                        line.GeometryCurve.GetEndPoint(0).X,
                        line.GeometryCurve.GetEndPoint(0).Y,
                        line.GeometryCurve.GetEndPoint(1).X,
                        line.GeometryCurve.GetEndPoint(1).Y
                    });
                }
            }
            p.CreatePattern(write);
            List<double[]> points = new List<double[]>();
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = doc.GetElement(lines[i]) as DetailLine;
                if (line != null)
                {
                    var pt = new double[4];
                    pt[0] = Math.Round(line.GeometryCurve.GetEndPoint(0).X, 3);
                    pt[1] = Math.Round(line.GeometryCurve.GetEndPoint(0).Y, 3);
                    pt[2] = Math.Round(line.GeometryCurve.GetEndPoint(1).X, 3);
                    pt[3] = Math.Round(line.GeometryCurve.GetEndPoint(1).Y, 3);
                    points.Add(pt);
                }
            }
            var ext = GetExtents(points);
            var text = new List<string>();
            text.Add("*Title");
            text.Add(";%TYPE=MODEL,");
            foreach (var pt in points)
                text.Add(GetText(pt, ext.Item2));
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
                text[0] = "*" + fp.Split('\\').Last().Split('.').First() + "," + ext.Item1;
                File.WriteAllLines(fp, text);
            }
        }
        private static Tuple<double, double[]> GetExtents(List<double[]> Points)
        {
            double[] extents = new double[4] { Points[0][0], Points[0][1], Points[0][2], Points[0][3] };
            for (int i = 1; i < Points.Count(); i++)
            {
                extents[0] = Math.Min(extents[0], Math.Min(Points[i][0], Points[i][2]));
                extents[1] = Math.Min(extents[1], Math.Min(Points[i][1], Points[i][3]));
                extents[2] = Math.Max(extents[2], Math.Max(Points[i][0], Points[i][2]));
                extents[3] = Math.Max(extents[3], Math.Max(Points[i][1], Points[i][3]));
            }
            var scalar = extents[3] - extents[1] > extents[2] - extents[0] ?
                12 * (extents[3] - extents[1]) :
                12 * (extents[2] - extents[0]);
            return new Tuple<double, double[]> (scalar, extents);
        }
        private static double[] Reframe(double[] point, double[] extents)
        {
            var maxx = extents[2] - extents[0];
            var maxy = extents[3] - extents[1];
            var max = Math.Max(maxx, maxy);

            var minx = extents[0];
            var miny = extents[1];

            return new double[4]
            {
                (point[0] - minx) / max,
                (point[1] - miny) / max,
                (point[2] - minx) / max,
                (point[3] - miny) / max
            };
        }

        private static string GetText(double[] _Line, double[] extents)
        {
            string line = "";

            var ln = Reframe(_Line, extents);

            var Dist = Length(ln);
            var AngTo = Angle(ln);
            var AngFrom = InvAngle(ln);

            bool IsValid = false;
            double DeltaX;
            double DeltaY;
            var Gap = Dist - 1;

            if (Math.Abs(ln[0] - ln[2]) < 0.001 || Math.Abs(ln[3] - ln[1]) < 0.001)
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
                var XDir = Math.Abs(ln[2] - ln[0]);
                var YDir = Math.Abs(ln[3] - ln[1]);
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
                line += Math.Round(AngTo * 180 / Math.PI, 6) + ",";
                line += Math.Round(ln[0], 8) + ",";
                line += Math.Round(ln[1], 8) + ",";
                line += Math.Round(DeltaX, 8) + ",";
                line += Math.Round(DeltaY, 8) + ",";
                line += Math.Round(Dist, 8) + ",";
                line += Math.Round(Gap, 8);
            }
            return line;
        }

        private static double Length(double[] Line)
        {
            var x = Line[2] - Line[0];
            var y = Line[3] - Line[3];
            return Math.Sqrt((x * x) + (y * y));
        }
        private static double Angle(double[] line) { return Math.Atan2(line[3] - line[1], line[2] - line[0]); }
        private static double InvAngle(double[] line) { return Math.Atan2(line[1] - line[3], line[0] - line[2]); }
    }
}
