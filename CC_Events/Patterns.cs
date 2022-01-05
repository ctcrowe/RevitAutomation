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
	private static double[] Reframe(double[] point, double[] extents)
        {
            var maxx = extents[2] - extents[0];
            var maxy = extents[3] - extents[1];
            var max = Math.Max(maxx, maxy);

            var minx = extents[0];
            var miny = extents[1];

            var ang = Angle(point);
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
        private static string GetText(double[] _Line, double[] extents)
        {
			string line = "";
			
		    var ln = Reframe(_Line, extents);
			var Dist = Length(ln);
			var AngTo = Angle(ln);
			var AngFrom = InvAngle(ln);
			
	    	bool IsValid = false;
		    var DeltaX = 0;
			var DeltaY = 0;
			var Gap = Dist - 1;
		
		    if(Math.Abs(ln[0] - ln[2]) < 0.001 || Math.Abs(ln[3] - ln[1]) < 0.001)
	    	{
				DeltaX = 0;
				DeltaY = 1;
				Gap = Dist - 1;
	    	    IsValid = true;
		    }
			
			var Ang = AngTo < Math.PI ? AngTo : AngFrom;
			var AngZone = Math.Floor(Ang / (Math.PI / 4));
			var XDir = Math.Abs(pt2.X - pt1.X);
			var YDir = Math.Abs(pt2.Y - pt1.Y);
			double Factor = 1;
			double RF = 1;
			
			Switch(AngZone)
			{
				default:
				case 0:
					DeltaY = Math.Abs(Sin(Ang));
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
			if(Math.Abs(XDir - YDir) > 0.001)
			{
				double Ratio = XDir < YDir ? YDir / XDir : XDir / YDir;
				RF = Ratio * Factor;
				var Scaler = XDir < YDir ? 1 / XDir : 1 / YDir;
				if(Ratio % 1 > 0.001 && 1 - (Ratio % 1) > 0.001)
				{
					While(Factor <= 100 && RF % 1 > 0.001 && 1 - RF % 1 > 0.001)
					{
						Factor++;
						RF = Ratio * Factor;
					}
					if(Factor > 1 && Factor <= 100)
					{
						var AB = XDir * Scaler * Factor;
						var BC = YDir * Scaler * Factor;
						var AC = Math.Sqrt((AB * AB) + (BC * BC))
						var EF = 1;
						double x = 1;
						While(x < AB - 0.5)
						{
							var y = x * YDir / XDir;
							var h = Ang < pi / 2 ? 1 - (y % 1) : y % 1;

							if(h < EF)
							{
								var AD = x;
								var DE = y;
								var AE = Math.Sqrt((x * x) + (y * y));
								var EF = h;
							}
							x++;
						}

						if(EF < 1)
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
				if(Factor == 1)
				{
					Gap = Dist - Math.Abs(Factor * 1 / DeltaY);
					IsValid = true;
				}
			}
			if(IsValid)
			{
				line += Math.Round(AngTo * 180 / Math.PI, 6) + ",";
				line += Math.Round(X, 8) + ",";
				line += Math.Round(Y, 8) + ",";
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
   	/*
	private static string GetText(double[] point, double[] extents)
        {
            var pt = Reframe(point, extents);
            return
                Angle(pt) + "," +
                OriginX(pt) + "," +
                OriginY(pt) + "," +
                ShiftX(pt) + "," +
                ShiftY(pt) + "," +
                Length(pt) + "," +
                Gap(pt);
        }
        private static double Angle(double[] line) { return Math.Round(180 * Math.Atan2(line[3] - line[1], line[2] - line[0]) / Math.PI); }
        private static double OriginX(double[] line) { return Math.Round(line[0], 6); }
        private static double OriginY(double[] line) { return Math.Round(line[0], 6); }
        private static double ShiftX(double[] line)
        {
            var H = Math.Sqrt(2);
            var dir = Angle(line);
            var ang = 45 - dir;
            var X = H * Math.Cos(ang * Math.PI / 180);
            return X == 0 ? H : X;
        }
        private static double ShiftY(double[] line)
        {
            var H = Math.Sqrt(2);
            var dir = Angle(line);
            var ang = 45 - dir;
            var Y = H * Math.Sin(ang * Math.PI / 180);
            return Y == 0 ? H : Y;
        }
        private static double Gap(double[] line)
        {
            var ang = Math.Atan2(line[3] - line[1], line[2] - line[0]);
            var a2 = ang * Math.PI / 180;
            if (a2 == 0 || a2 == 90 || a2 == -90)
                return Length(line) - 1;

            var yprime = Math.Abs(Math.Tan(ang));
            var zy = LCM(yprime, 1);
            var hyp1 = zy / Math.Abs(Math.Sin(ang));

            var xprime = 1 / Math.Abs(Math.Tan(ang));
            var zx = LCM(xprime, 1);
            var hyp2 = zx / Math.Abs(Math.Cos(ang));
            
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
            return a > b ?
                (a * b) / GCD(a, b) : 
                (a * b) / GCD(b, a);
        }
        private static int[] CreateIntegers(double x, double y, int z = 0)
        {
            return x % 10 == 0 && y % 10 == 0 ?
                new int[3] {(int)(x / 10), (int)(y / 10), z - 1} :
                CreateIntegers(x * 10, y * 10, z++);
        }
    }
}
*/
