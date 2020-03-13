namespace CC_Library.Predictions
{
    internal class Data
    {
        public string Phrase;
        private int[] Dataset;
        public Data(string phrase)
        {
            this.Phrase = phrase;
            this.Dataset = new int[20];
            Random rnd = new Random();
            for(int i = 0; i < Dataset.Count(); i++)
            {
                Dataset[i] = rnd.Next(1, 100);
            }
        }
        public void SetValue(int i, int value)
        {
            Dataset[i] = value;
        }
        public int GetValue(int i)
        {
            return Dataset[i];
        }
    }
}