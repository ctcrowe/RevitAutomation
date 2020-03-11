using System;
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
}
