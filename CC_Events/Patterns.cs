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
            var dir = GetAngle(pt);
            var origin = GetOrigin(pt);
            var shift = GetShift(pt);
            var pendown = Length(pt);
            var penup = -1 * RepLength(pt, extents);

            return
                dir + ", " + Math.Round(origin[0], 6) + ", " + Math.Round(origin[1], 6) + ", " +
                Math.Round(shift[0], 6) + ", " + Math.Round(shift[1], 6) + ", " + Math.Round(pendown, 6) + ", " + Math.Round(penup, 6);
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
        private static double GetAngle(double[] line)
        {
            var angle = 180 * Math.Atan2(line[3] - line[1], line[2] - line[0]) / Math.PI;
            angle = Math.Round(angle);
            return angle;
        }
        private static double[] GetOrigin(double[] line) { return new double[2] { line[0], line[1] }; }
        private static double[] GetShift(double[] line)
        {
            var X = Math.Sin(ang * Math.PI / 180);
            X = X == 0 ? 1 : X;
            var Y = Math.Cos(ang * Math.PI / 180);
            Y = Y == 0 ? 1 : Y;
            return new double[2] { X, Y };
            /*
            var H = Math.Sqrt(2);
            var dir = GetAngle(line);
            var ang = 45 - dir;
            var X = H * Math.Cos(ang * Math.PI / 180);
            X = X == 0 ? H : X;
            var Y = H * Math.Sin(ang * Math.PI / 180);
            Y = Y == 0 ? H : Y;
            return new double[2] { X, Y };
            */
        }
        private static double RepLength(double[] line, double[] extents)
        {
            var ang = GetAngle(line);
            if (ang == 0 || ang == 90 || ang == -90)
                return Length(line) - 1;

            //distance across the length of the pattern that the line is
            var yprime = Math.Tan(ang * Math.PI / 180);
            //var z = FindSmallestMultiplier(yprime, 5e-3);
            var z = LCM(yprime, 1);
            return z - Length(line);
        }
        private static double Length(double[] point)
        {
            var x = (point[2] - point[0]) * (point[2] - point[0]);
            var y = (point[3] - point[1]) * (point[3] - point[1]);
            return Math.Sqrt(x + y);
        }
        // Reconstructs a fraction from a continued fraction with the given coefficients
        public static double GCD(double a, double b)
        {
            return !b ? a : GCD(b, a % b);
        }
        public static double LCM(double a, double b)
        {
            return (a * b) / GCD(a, b);
        }
        private static int[] CreateIntegers(double x, double y, int z == 0)
        {
            Return x % 10 == 0 && y % 10 == 0 ?
                new int[3] {(int)(x / 10), (int)(y / 10), z} :
                CreateIntegers(x * 10, y * 10, z++);
        }
        private static Tuple<int, int> ReconstructContinuedFraction(List<int> coefficients)
        {
            int numerator = coefficients.Last();
            int denominator = 1;

            for(int i = coefficients.Count - 2; i >= 0; --i)
            {
                //swap numerator and denominator (= invert number)
                var temp = numerator;
                numerator = denominator;
                denominator = temp;

                numerator += denominator * coefficients[i];
            }
            return new Tuple<int, int>(numerator, denominator);
        }
        private static int FindSmallestMultiplier(double input, double error)
        {
            double remainingToRepresent = input;
            List<int> coefficients = new List<int>();
            while (true)
            {
                //calculate the next coefficient
                var integer = (int)Math.Floor(remainingToRepresent);                
                remainingToRepresent -= integer;
                remainingToRepresent = 1 / remainingToRepresent;
                coefficients.Add(integer);

                //check if we reached the desired accuracy
                var reconstructed = ReconstructContinuedFraction(coefficients);

                var multipliedInput = input * reconstructed.Item2;
                var multipliedInputRounded = Math.Round(multipliedInput);
                if (Math.Abs(multipliedInput - multipliedInputRounded) < error)
                    return reconstructed.Item2;
            }
        }
    }
}
/*
function leastCommonMultiple(min, max) {
    function range(min, max) {
        var arr = [];
        for (var i = min; i <= max; i++) {
            arr.push(i);
        }
        return arr;
    }

    function gcd(a, b) {
        return !b ? a : gcd(b, a % b);
    }

    function lcm(a, b) {
        return (a * b) / gcd(a, b);   
    }

    var multiple = min;
    range(min, max).forEach(function(n) {
        multiple = lcm(multiple, n);
    });

    return multiple;
}

leastCommonMultiple(1, 13); // => 360360
*/
