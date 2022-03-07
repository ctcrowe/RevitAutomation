namespace CC_Library.Predictions
{
    internal interface IAlphaFilter
    {
        NeuralNetwork AttentionNetwork { get; }
        NeuralNetwork ValueNetwork { get; }
        int GetSize();
        int GetLength(string s, NeuralNetwork net);
        double GetChangeSize();
        double[][][][][] Forward(string s, NeuralNetwork net)
        void Backward(string s, double[] DValues,
            AlphaMem am, NetworkMem mem, NetworkMem CtxtMem, NeuralNetwork net);
    }
}
