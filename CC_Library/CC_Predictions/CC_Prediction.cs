namespace CC_Library
{

    internal class Prediction
    {
        public string Name { get; set; }
        public double Value { get; set; }
        private int Count { get; set; }

        public Prediction(string s)
        {
            this.Name = s;
            this.Value = 0;
            this.Count = 0;
        }
        public Prediction( PredictionOption o)
        {
            this.Name = o.Name;
            this.Count = 1;
            double adj;
            if(o.Negative > o.Positive)
                adj = -1;
            else
                adj = 1
            double v = Math.Abs(o.Positive - o.Negative);

        }
        public void Combine(PredictionOption o)
        {
        }
        private static double CalcAdjustment(PredictionOption o)
        {
            return 0;
        }
    }
}
