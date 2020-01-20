namespace CC_Library
{
    public class PredictionElement
    {
        public string Word { get; }
        public int[] Predictions { get; set; }

        public PredictionElement(string s)
        {
            this.Word = s;
            this.Predictions = new int[26];
        }
    }
}
