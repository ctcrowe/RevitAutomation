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
        }
        public void Combine(PredictionOption o)
        {
        }
    }
}
