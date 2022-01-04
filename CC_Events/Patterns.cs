using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Autodesk.Revit.DB;
using System.Windows.Forms;
using CC_Library.Datatypes;

namespace CC_Plugin
{
    //Reframe the hatch from 0 to 1, include a comment that tells the user what to scale it to!!!!
    public class HatchEditor
    {
        public static void EditHatch(Document doc)
        {
            var v = doc.ActiveView;
            var lines = new FilteredElementCollector(doc, v.Id).OfCategory(BuiltInCategory.OST_Lines).ToElementIds().ToList();
            List<double[]> points = new List<double[]>();
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = doc.GetElement(lines[i]) as DetailLine;
                if (line != null)
                {
                    var pt = new double[4];
                    pt[0] = Math.Round(line.GeometryCurve.GetEndPoint(0).X, 6);
                    pt[1] = Math.Round(line.GeometryCurve.GetEndPoint(0).Y, 6);
                    pt[2] = Math.Round(line.GeometryCurve.GetEndPoint(1).X, 6);
                    pt[3] = Math.Round(line.GeometryCurve.GetEndPoint(1).Y, 6);
                    points.Add(pt);
                }
            }
            var ext = GetExtents(points);
            var text = new List<string>();
            text.Add("*Title");
            text.Add(";%TYPE=MODEL,");
            foreach (var pt in points)
                text.Add(GetText(pt, ext));
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
                if(!fp.EndsWith(".pat"))
                    fp += ".pat";
                text[0] = "*" + fp.Split('\\').Last().Split('.').First();
                File.WriteAllLines(fp, text);
            }
        }
        private static double[] GetExtents(List<double[]> Points)
        {
            double[] extents = new double[4] { Points[0][0], Points[0][1], Points[0][2], Points[0][3] };
            for(int i = 1; i < Points.Count(); i++)
            {
                extents[0] = Math.Min(extents[0], Math.Min(Points[i][0], Points[i][2]));
                extents[1] = Math.Min(extents[1], Math.Min(Points[i][1], Points[i][3]));
                extents[2] = Math.Max(extents[2], Math.Max(Points[i][0], Points[i][2]));
                extents[3] = Math.Max(extents[3], Math.Max(Points[i][1], Points[i][3]));
            }
            return extents;
        }
        private static string GetText(double[] point, double[] extents)
        {
            var pt = Reframe(point, extents);
            return
                GetAngle(pt) + "," +
                OriginX(pt) + "," +
                OriginY(pt) + "," +
                ShiftX(pt) + "," +
                ShiftY(pt) + "," +
                Length(pt) + "," +
                Gap(pt);
        }
        private static double[] Reframe(double[] point, double[] extents)
        {
            var maxx = extents[2] - extents[0];
            var maxy = extents[3] - extents[1];
            var max = Math.Max(maxx, maxy);

            var minx = extents[0];
            var miny = extents[1];

            var ang = GetAngle(point);
            if(ang > 90 || ang < -90)
            {
                return new double[4]
                {
                    (point[2] - minx) / max,
                    (point[3] - miny) / max,
                    (point[0] - minx) / max,
                    (point[1] - miny) / max
                };
            }
            return new double[4]
            {
                (point[0] - minx) / max,
                (point[1] - miny) / max,
                (point[2] - minx) / max,
                (point[3] - miny) / max
            };
        }
        private static double Angle(double[] line) { return Math.Round(180 * Math.Atan2(line[3] - line[1], line[2] - line[0]) / Math.PI); }
        private static double OriginX(double[] line) { return Math.Round(line[0], 6); }
        private static double OriginY(double[] line) { return Math.Round(line[0], 6); }
        private static double ShiftX(double[] line) { return Math.Sin(Angle(line) * Math.PI / 180) == 0 ? 1 : Math.Round(Math.Sin(Angle(line) * Math.PI / 180), 6); }
        private static double ShiftY(double[] line) { return Math.Cos(Angle(line) * Math.PI / 180) == 0 ? 1 : Math.Round(Math.Cos(Angle(line) * Math.PI / 180), 6); }
        private static double Gap(double[] line)
        {
            var ang = Math.Atan2(line[3] - line[1], line[2] - line[0]);
            var a2 = ang * Math.PI / 180;
            if (a2 == 0 || a2 90 || a2 == -90)
                return Length(line) - 1;
            
            var yprime = Math.Tan(ang);
            var zy = LCM(yprime, 1);
            var hyp1 = zy / Math.Sin(ang);
            
            var xprime = 1 / Math.Tan(ang);
            var zx = LCM(xprime, 1);
            var hyp2 = zx / Math.Cos(ang);
            
            return hyp1 < hyp2 ? Length(line) - hyp1 : Length(line) - hyp2;
        }
        private static double Length(double[] point)
        {
            var x = (point[2] - point[0]) * (point[2] - point[0]);
            var y = (point[3] - point[1]) * (point[3] - point[1]);
            return Math.Round(Math.Sqrt(x + y), 6);
        }
        public static double GCD(double a, double b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
        public static double LCM(double a, double b)
        {
            var ints = CreateIntegers(a, b);
            return a > b ?
                (ints[0] * ints[1]) / GCD(ints[0], ints[1]) : 
                (ints[0] * ints[1]) / GCD(ints[1], ints[0]);
        }
        private static int[] CreateIntegers(double x, double y, int z = 0)
        {
            return x % 10 == 0 && y % 10 == 0 ?
                new int[3] {(int)(x / 10), (int)(y / 10), z - 1} :
                CreateIntegers(x * 10, y * 10, z++);
        }
    }
}
