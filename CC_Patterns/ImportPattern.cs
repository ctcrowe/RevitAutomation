using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CC_Patterns
{
    internal static class ImportPattern_CMD
    {
        public static void ImportPattern(this Document doc, string name, List<string> lines)
        {
            using (Transaction t = new Transaction(doc, "Create Grids"))
            {
                t.Start();
                List<FillGrid> grids = new List<FillGrid>();
                FillPattern pat = new FillPattern(name, FillPatternTarget.Model, FillPatternHostOrientation.ToHost);
                foreach (string line in lines)
                {
                    FillGrid grid = new FillGrid();
                    grid.Angle = double.Parse(line.Split(',')[0]) * Math.PI / 180;
                    grid.Origin = new UV(double.Parse(line.Split(',')[1]) / 12, double.Parse(line.Split(',')[2]) / 12);
                    grid.Shift = double.Parse(line.Split(',')[3]) / 12;
                    grid.Offset = double.Parse(line.Split(',')[4]) / 12;
                    var segments = new List<double>();
                    segments.Add(Math.Abs(double.Parse(line.Split(',')[5])) / 12);
                    segments.Add(Math.Abs(double.Parse(line.Split(',')[6])) / 12);
                    grid.SetSegments(segments);
                    grids.Add(grid);
                }

                pat.SetFillGrids(grids);
                FillPatternElement fpe = FillPatternElement.Create(doc, pat);
                t.Commit();
            }
        }
        public static void AddLine(this List<string> lines, double Angle, double X, double Y, double ShiftX, double ShiftY, double PenDown, double PenUp)
        {
            string s = Angle.ToString() + ",";
            s += X + ",";
            s += Y + ",";
            s += ShiftX + ",";
            s += ShiftY + ",";
            s += PenDown + ",";
            var PU = -1 * Math.Abs(PenUp);
            s += PU;
            lines.Add(s);
        }
    }
}
