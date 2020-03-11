using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal class Solution
    {
        private string DataName;
        private string SolutionName;
        private string SolutionValue;

        public Solution(string dataname, string solutionname, string solutionvalue)
        {
            this.DataName = dataname;
            this.SolutionName = solutionname;
            this.SolutionValue = solutionvalue;
        }
    }
    internal static class CMDGetSolution
    {
        public static Solution GetSolution(this string fn)
        {
            if(File.Exists(fn))
            {
                string[] lines = File.ReadAllLines(fn);
                if(lines.Count() >= 4)
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
