using System.Linq;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class CMDGetSolution
    {
        public static Solution GetSolution(this string fn)
        {
            if (File.Exists(fn))
            {
                string[] lines = File.ReadAllLines(fn);
                if (lines.Count() >= 4)
                {
                    return new Solution(lines[2].Split('\t').First(),
                                        lines[1].Split('\t').First(),
                                        lines[3].Split('\t').First());
                }
            }
            return new Solution("null", "null", "null");
        }
    }
}
