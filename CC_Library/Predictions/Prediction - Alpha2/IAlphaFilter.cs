namespace CC_Library.Predictions
{
    internal interface IAlphaFilter
    {
        NeuralNetwork AttentionNetwork { get; }
        NeuralNetwork ValueNetwork { get; }
        const int Size;
        double[] Forward(string s);
        void Backward(string s, double[] DValues,
            AlphaMem am, NetworkMem mem, NetworkMem CtxtMem);
    }
}
