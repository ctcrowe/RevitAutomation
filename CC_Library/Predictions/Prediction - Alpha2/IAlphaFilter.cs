namespace CC_Library.Predictions
{
    internal interface IAlphaFilter
    {
        NeuralNetwork[] Networks { get; }
        string Name { get; }
        int GetSize();
        int GetLength(string s, NeuralNetwork net);
        double GetChangeSize();
        double[][][][][] Forward(string s, NeuralNetwork net);
        void Backward(double[] DValues, double[][][][][] outputs, NetworkMem[] mem, WriteToCMDLine write, bool tf = false);
    }
}
