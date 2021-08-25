using System.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class Accuracy
    {
        public double Acc;
        private double[] Distance;
        private int[] predicted;
        private int[] actual;
        private int[] Correct;
        private double[] Error;

        public Accuracy(Sample[] s)
        {
            int Count = s.Count();

            Acc = 0;
            Distance = new double[Count];
            Correct = new int[Count];
            actual = new int[Count];
            predicted = new int[Count];
            Error = new double[Count];
        }

        public void Add(string input, int numb, double val, double error, int pred)
        {
            predicted[numb] = pred;
            if (pred == actual[numb]) Correct[numb] = 1;
            else Correct[numb] = 0;
            Distance[numb] = val;
            Error[numb] = error;
        }
        public string[] Get()
        {
            string[] s = new string[13];
            Acc = 1.0 * Correct.Sum() / Correct.Count();
            s[0] = "Min Confidence : " + Distance.Min();
            s[1] = "Max Confidence : " + Distance.Max();
            s[2] = "Average Confidence : " + (Distance.Sum() / Distance.Count());
            s[3] = "";
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
