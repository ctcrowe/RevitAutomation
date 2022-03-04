namespace CC_Library.Predictions
{
    internal interface IAlphaFilter
    {
        NeuralNetwork AttentionNetwork { get; }
        NeuralNetwork ValueNetwork { get; }
        int GetSize();
        double GetChangeSize();
        double[] Forward(string s, AlphaMem am, NeuralNetwork net);
        void Backward(string s, double[] DValues,
            AlphaMem am, NetworkMem mem, NetworkMem CtxtMem, NeuralNetwork net);
    }
}
