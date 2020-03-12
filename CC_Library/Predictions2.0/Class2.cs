using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CC_Library.Predictions
{
    public static class CreateFiles
    {
        public static void Create(this Datafile df)
        {
            string dir = "";
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir);
                Solution[] solutions = new Solution[files.Count()];

                for(int i = 0; i < files.Count(); i++)
                {
                    solutions[i] = files[i].GetSolution();
                }

                List<string> words = solutions.GetPhrases();
                
            }
        }
        internal static List<string> GetPhrases(this Solution[] sols)
        {
            List<string> phrases = new List<string>();
            foreach(Solution s in sols)
            {
                if (!phrases.Any(x => x == s.DataName()))
                    phrases.Add(s.DataName());
            }
            return phrases;
        }
    }
}
