namespace CC_Library.Predictions
{
    internal class Solution
    {
        private string _DataName;
        private string _SolutionName;
        private string _SolutionValue;

        public Solution(string dataname, string solutionname, string solutionvalue)
        {
            this._DataName = dataname;
            this._SolutionName = solutionname;
            this._SolutionValue = solutionvalue;
        }
        public string DataName()
        {
            return _DataName;
        }
        public string SolutionName()
        {
            return _SolutionName;
        }
        public string SolutionValue()
        {
            return _SolutionValue;
        }
    }
}
