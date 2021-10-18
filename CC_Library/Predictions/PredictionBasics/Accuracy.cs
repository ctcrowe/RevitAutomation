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

/*
    public class Accuracy
    {
        public double Acc;
        private string[] inputs;
        private double[] Distance;
        private int[] predicted;
        private int[] actual;
        private int[] Correct;
        private double[] Error;
        private double StoredError;
        private double StartingError;
        private int Completion;

        public Accuracy(string[] Lines)
        {
            int Count = Lines.Count();

            Acc = 0;
            Completion = 0;
            inputs = new string[Count];
            Distance = new double[Count];
            Correct = new int[Count];
            actual = new int[Count];
            predicted = new int[Count];
            Error = new double[Count];
            StoredError = double.MaxValue;
            StartingError = double.MaxValue;

            Parallel.For(0, Count, i => actual[i] = int.Parse(Lines[i].Split(',').Last()));
        }

        public void Add(string input, int numb, double val, double error, int pred)
        {
            inputs[numb] = input;
            predicted[numb] = pred;
            if (pred == actual[numb]) Correct[numb] = 1;
            else Correct[numb] = 0;
            Distance[numb] = val;
            Error[numb] = error;
            Completion++;
        }
        public string[] Get()
        {
            string[] s = new string[13];
            Acc = 1.0 * Correct.Sum() / Correct.Count();
            int MinDist = Distance.ToList().IndexOf(Distance.Min());
            int MaxDist = Distance.ToList().IndexOf(Distance.Max());
            int MinError = Error.ToList().IndexOf(Error.Min());
            int MaxError = Error.ToList().IndexOf(Error.Max());

            s[0] = "Min Confidence : " + Distance.Min() + " : " + inputs[MinDist] + " : " + predicted[MinDist] + " : " + actual[MinDist];
            s[1] = "Max Confidence : " + Distance.Max() + " : " + inputs[MaxDist] + " : " + predicted[MaxDist] + " : " + actual[MaxDist];
            s[2] = "Average Confidence : " + (Distance.Sum() / Distance.Count());
            s[3] = "";
            s[4] = "Min Error : " + Error.Min() + " : " + inputs[MinError] + " : " + predicted[MinError] + " : " + actual[MinError];
            s[5] = "Max Error : " + Error.Max() + " : " + inputs[MaxError] + " : " + predicted[MaxError] + " : " + actual[MaxError];
            s[6] = "Average Error : " + (Error.Sum() / Error.Count());
            s[7] = "Stored Error : " + StoredError;
            s[8] = "Starting Error : " + StartingError;
            s[9] = "";
            s[10] = "Accuracy : " + Correct.Sum() + " / " + Correct.Count() + " = " + Acc;
            s[11] = "Percent Complete : " + Completion * 1.0 / inputs.Count();
            s[12] = "";

            return s;
        }
        public void Show(WriteToCMDLine write)
        {
            Acc = 1.0 * Correct.Sum() / Correct.Count();
            int MinDist = Distance.ToList().IndexOf(Distance.Min());
            int MaxDist = Distance.ToList().IndexOf(Distance.Max());
            int MinError = Error.ToList().IndexOf(Error.Min());
            int MaxError = Error.ToList().IndexOf(Error.Max());

            write("Min Confidence : " + Distance.Min() + " : " + inputs[MinDist] + " : " + predicted[MinDist] + " : " + actual[MinDist]);
            write("Max Confidence : " + Distance.Max() + " : " + inputs[MaxDist] + " : " + predicted[MaxDist] + " : " + actual[MaxDist]);
            write("Average Confidence : " + (Distance.Sum() / Distance.Count()));
            write("");
            write("Min Error : " + Error.Min() + " : " + inputs[MinError] + " : " + predicted[MinError] + " : " + actual[MinError]);
            write("Max Error : " + Error.Max() + " : " + inputs[MaxError] + " : " + predicted[MaxError] + " : " + actual[MaxError]);
            write("Average Error : " + (Error.Sum() / Error.Count()));
            write("Stored Error : " + StoredError);
            write("Starting Error : " + StartingError);
            write("");
            write("Accuracy : " + Correct.Sum() + " / " + Correct.Count() + " = " + Acc);
            write("Percent Complete : " + Completion * 1.0 / inputs.Count());
        }
        public bool CheckError()
        {
            double e = Error.Sum() / Error.Count();
            if(e < StoredError)
            {
                StoredError = e;
                return true;
            }
            return false;
        }
        public double RoughError()
        {
            return Error.Sum() / Error.Count();
        }
        public void SetAcc()
        {
            Acc = 1.0 * Correct.Sum() / Correct.Count();
        }
        public void Reset()
        {
            Completion = 0;
        }
        public void SetStartingError()
        {
            StartingError = Error.Sum() / Error.Count();
        }
    }
}
*/
