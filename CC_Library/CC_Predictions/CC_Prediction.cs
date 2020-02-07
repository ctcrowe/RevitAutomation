namespace CC_Library
{

    internal class Prediction
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        private int Count { get; set; }

        public Prediction(string s)
        {
            this.Name = s;
            this.Weight = 0;
            this.Count = 0;
        }
        public void Adjust(double d)
        {
            double x = this.Weight * this.Count;
            this.Count++;
            x += d;
            this.Weight = x / this.Count;
        }
    }
}
