using System.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class Accuracy
    {
        public double Acc;
        private int[] Correct;
        private double[] Error;

        public Accuracy(Sample[] s)
        {
            int Count = s.Count();

            Acc = 0;
            Correct = new int[Count];
            Error = new double[Count];
        }

        public void Add(int numb, double error, int pred, int actual)
        {
            if (pred == actual) Correct[numb] = 1;
            else Correct[numb] = 0;
            Error[numb] = error;
        }
        public string[] Get()
        {

            string[] s = new string[5];
            Acc = 1.0 * Correct.Sum() / Correct.Count();
            s[0] = "Min Error : " + Error.Min();
            s[1] = "Max Error : " + Error.Max();
            s[2] = "Average Error : " + (Error.Sum() / Error.Count());
            s[3] = "Accuracy : " + Correct.Sum() + " / " + Correct.Count() + " = " + Acc;
            s[4] = "";

            return s;
        }
    }
}
