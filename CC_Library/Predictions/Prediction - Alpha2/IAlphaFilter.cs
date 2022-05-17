namespace CC_Library.Predictions
{
    internal interface IAlphaTransformer
    {
        public string Name {get; set;}
        public int Size {get; set;}
        public double[,] Queries { get; set; }
        public double[,] Keys { get; set; }
        public double[,] Values { get; set; }
    }
    internal interface IAlphaFilter
    {
        NeuralNetwork[] Networks { get; }
        string Name { get; }
        int GetSize();
        double GetChangeSize();
        double[][][][][] Forward(string s);
        void Backward(double[] DValues, double[][][][][] outputs, NetworkMem[] mem, WriteToCMDLine write, bool tf = false);
    }
}
