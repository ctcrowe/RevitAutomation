namespace CC_Library.Predictions
{
    internal interface IAlphaTransformer
    {
        string Name {get;}
        int Size {get;}
        double[,] Queries { get; set; }
        double[,] Keys { get; set; }
        double[,] Values { get; set; }
        
        void Forward(string s, AttentionMem mem);
        void Backward(AttentionMem mem, AttentionChange change, double[] dvals);
        double[] Forward(string s);
    }
}
