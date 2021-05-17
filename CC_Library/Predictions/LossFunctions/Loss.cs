namespace CC_Library.Predictions
{
    internal class CyclicError
    {
        public int run { get; set;}
        public double CurError { get; set; }
        public double TotError { get; set; }
        public double RunError { get; set; }
        public double PrevError { get; set; }
        public double Travel { get; set; }

        public CyclicError()
        {
            run = 0;
            TotError = 0;
            RunError = 0;
            CurError = 0;
            PrevError = 0;
            Travel = 0;
        }
        public void Add(double error)
        {
            CurError += error;
            TotError += error;
            run++;
            RunError = TotError / run;
        }
        public void Show(WriteToCMDLine write)
        {
            write("Current Error " + CurError);
            write("Run Error " + RunError);
            write("Previous Error " + PrevError);
            write("Travel " + Travel);
            CurError = 0;
        }
        public void Reset()
        {
            Travel /= 2;
            if (PrevError != 0)
                Travel += (RunError - PrevError);
            PrevError = RunError;
            CurError = 0;
            TotError = 0;
            RunError = 0;
            run = 0;
        }
    }
}
