using System.Linq;
using System.IO;

namespace CC_Library.Predictions
{
    //collects all solutions for a given Dataset
    internal static class CMDGetSolutions
    {
        public static Solution[] GetSolutions(this DataFile df)
        {
            string dir = df.ToString().GetMyDocs();
            string datafile = df.ToString().GetMyDocs() + ".xml";
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir);
                Solution[] solutions = new Solution[files.Count()];

                for (int i = 0; i < files.Count(); i++)
                {
                    solutions[i] = files[i].GetSolution();
                }
                return solutions;
            }
            return null;
        }
    }
}
