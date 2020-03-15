using System.Linq;
using System.IO;

namespace CC_Library.Predictions
{
    //collects all solutions for a given Dataset
    internal static class CMDGetSolutions
    {
        public static Solution[] GetSolutions(this DataFile df, CMDGetMyDocs.WriteOutput wo)
        {
            string dir = df.ToString().GetMyDocs(wo);
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
            else { Directory.CreateDirectory(dir); }
            return null;
        }
    }
}
