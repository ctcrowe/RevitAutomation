namespace CC_Library.Predictions
{
    internal interface IAlphaFilter
    {
        NeuralNetwork AttentionNetwork { get; }
        NeuralNetwork ValueNetwork { get; }
        string Name { get; }
        int GetSize();
        int GetLength(string s, NeuralNetwork net);
        double GetChangeSize();
        double[][][][][] Forward(string s, NeuralNetwork net);
        void Backward(double[] DValues, double[][][][][] outputs, NetworkMem ValMem, NetworkMem FocMem, WriteToCMDLine write, bool tf = false);
    }
}
