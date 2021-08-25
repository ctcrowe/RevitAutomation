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
            predicted[numb] = pred;
            if (pred == actual) Correct[numb] = 1;
            else Correct[numb] = 0;
            Error[numb] = error;
        }
        public string[] Get()
        {
            string[] s = new string[13];
            Acc = 1.0 * Correct.Sum() / Correct.Count();
            s[4] = "Min Error : " + Error.Min() + " : " + inputs[MinError];
            s[5] = "Max Error : " + Error.Max() + " : " + inputs[MaxError];
            s[6] = "Average Error : " + (Error.Sum() / Error.Count());
            s[9] = "";
            s[10] = "Accuracy : " + Correct.Sum() + " / " + Correct.Count() + " = " + Acc;
            s[12] = "";

            return s;
        }
    }
}
