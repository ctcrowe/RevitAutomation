namespace CC_Library.Predictions
{
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
