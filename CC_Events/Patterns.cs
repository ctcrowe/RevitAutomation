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
    public class HatchEditor
    {
        public void EditHatch(Document doc)
        {
            if (doc.OwnerFamily.FamilyCategory == Category.GetCategory(doc, BuiltInCategory.OST_DetailComponents))
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
                        pt[0] = line.GeometryCurve.GetEndPoint(0).X;
                        pt[1] = line.GeometryCurve.GetEndPoint(0).Y;
                        pt[2] = line.GeometryCurve.GetEndPoint(1).X;
                        pt[3] = line.GeometryCurve.GetEndPoint(1).Y;
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
                    FileName = "Create a txt file",
                    Filter = "TXT files (*.txt)|*.txt",
                    Title = "Create a txt file"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var fp = sfd.FileName;
                    text[0] = "*" + fp.Split('\\').Last().Split('.').First();
                    File.WriteAllLines(fp, text);
                }
            }
            else
            {
            }
        }
        private static double[] GetExtents(List<double[]> Points)
        {
            double[] extents = new double[4];
            for(int i = 0; i < Points.Count(); i++)
            {
                if (Points[i][0] < extents[0])
                    extents[0] = Points[i][0];
                if (Points[i][2] < extents[0])
                    extents[0] = Points[i][2];
                if (Points[i][1] < extents[1])
                    extents[1] = Points[i][1];
                if (Points[i][3] < extents[1])
                    extents[1] = Points[i][3];

                if (Points[i][0] > extents[2])
                    extents[2] = Points[i][0];
                if (Points[i][2] > extents[2])
                    extents[2] = Points[i][2];
                if (Points[i][1] > extents[3])
                    extents[3] = Points[i][1];
                if (Points[i][3] > extents[3])
                    extents[3] = Points[i][3];
            }
            return extents;
        }
        private static string GetText(double[] point, double[] extents)
        {
            var dir = GetAngle(point);
            var origin = GetOrigin(point);
            var shift = new double[2] { extents[0], extents[1] };
            var pendown = Length(point);
            var penup = -1 * IntersectLength(point, extents);

            return
                dir + ", " + origin[0] + ", " + origin[1] + ", " +
                shift[0] + ", " + shift[1] + ", " + pendown + ", " + penup;
        }
        private static double GetAngle(double[] line)
        {
            return 180 * Math.Atan2(line[3] - line[1], line[2] - line[0]) / Math.PI;
        }
        private static double[] GetOrigin(double[] line)
        {
            var dir = -1 * Math.Atan2(line[3] - line[1], line[2] - line[0]);
            var rotx = (line[0] * Math.Cos(dir)) - (line[1] * Math.Sin(dir));
            var roty = (line[1] * Math.Cos(dir)) + (line[0] * Math.Sin(dir));
            return new double[2] { rotx, roty };
        }
        private static double Length(double[] point)
        {
        }
        private static double IntersectLength(double[] point, double[] extents)
        {
            return 12;
        }
    }
}
/* hatch editor
// if(doc.IsFamilyDocument)
// {
//      if(doc.OwnerFamily.FamilyCategory == Detail Item)
//      {
            Get Line Start Point
            Get Line End Point
            Add to List<string>Lines();
            //Generate Title
            //Write All Lines to File
            //Profit
//      }
// }
*/
