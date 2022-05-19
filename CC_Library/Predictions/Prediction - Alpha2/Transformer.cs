using System;

namespace CC_Library.Predictions
{
    [Serializable]
    internal class Transformer
    {
        private const double Rate = 0.01;
        public string Name { get; set; }
        public int Size {get;}
        public int Radius { get; }
        double[,] Queries { get; set; }
        double[,] Keys { get; set; }
        double[,] Values { get; set; }

        #region overloads
        public Transformer(string _Name, int _Size, int _Radius = 1)
        {
            Name = _Name;
            Size = _Size;
            Radius = _Radius;
            Queries = new double[CharSet.CharCount * (1 + (2 * Radius)), Size];
            Keys = new double[CharSet.CharCount * (1 + (2 * Radius)), Size];
            Values = new double[CharSet.CharCount * (1 + (2 * Radius)), Size];
            Queries.SetRandom();
            Keys.SetRandom();
            Values.SetRandom();
        }
        #endregion

        public void Forward(string s, AttentionMem mem)
        {
            mem.input = s.Locate(Radius); //Size should be s.Length, CharCount * Diameter

            try { mem.Q = mem.input.Dot(Queries); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size
            try { mem.K = mem.input.Dot(Keys); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size
            try { mem.V = mem.input.Dot(Values); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size

            try { mem.scores = mem.Q.Dot(mem.K.Transpose()); }//Size should be s.Length, s.Length
            catch (Exception e) { e.OutputError(); }
            try { mem.weights = Activations.SoftMax(mem.scores); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, s.Length
            try { mem.attn = mem.weights.Dot(mem.V); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size

            try { mem.attention = mem.attn.SumRange(); } catch (Exception e) { e.OutputError(); } //Size should be size
        }
        public void Backward(AttentionMem mem, AttentionChange change, double[] dvals) //dvals Size is always size
        {
            var atndvals = dvals.Dot(mem.attn.Ones()); //returns a vector [s.Length, size]
            var Vdvals = mem.weights.Transpose().Dot(atndvals); //returns a vector [size, s.Length] - changed to [s.Length, size]
            var DV = mem.input.Transpose().Dot(Vdvals); //size is CharCount * diameter, size

            var dweights = mem.V.Dot(atndvals.Transpose()); // Size of this is s.Length, s.Length
            dweights = Activations.InverseSoftMax(dweights, mem.weights); // Size of this is s.Length, s.Length
            var Qdvals = dweights.Transpose().Dot(mem.K); //size is s.Length, size
            var Kdvals = dweights.Dot(mem.Q); //size is s.Length, size
            var DQ = mem.input.Transpose().Dot(Qdvals); //this needs to be CharCount * diameter, size
            var DK = mem.input.Transpose().Dot(Kdvals);

            change.Q.Add(DQ);
            change.K.Add(DK);
            change.V.Add(DV);
        }
        double[] Forward(string s)
        {
            var input = s.Locate(Radius); //Size should be s.Length, CharCount * Diameter
            var output = new double[Size];

            try
            {
                var Q = input.Dot(Queries); //Size should be s.Length, size
                var K = input.Dot(Keys); //Size should be s.Length, size
                var V = input.Dot(Values); //Size should be s.Length, size

                var scores = Q.Dot(K.Transpose()); //Size should be s.Length, s.Length
                var weights = Activations.SoftMax(scores); //Size should be s.Length, s.Length
                var attn = weights.Dot(V);  //Size should be s.Length, size
                output = attn.SumRange();
            }
            catch (Exception e) { e.OutputError(); }
            return output;
        }
        public void Update(AttentionChange change, int Count, WriteToCMDLine write)
        {
            Queries.Update(change.Q, Rate / Count);
            Keys.Update(change.K, Rate / Count);
            Values.Update(change.V, Rate / Count);
        }
    }
    internal class AttentionChange
    {
        public double[,] Q;
        public double[,] K;
        public double[,] V;

        public AttentionChange(Transformer xfmr)
        {
            this.Q = new double[CharSet.CharCount * (1 + (2 * xfmr.Radius)), xfmr.Size];
            this.K = new double[CharSet.CharCount * (1 + (2 * xfmr.Radius)), xfmr.Size];
            this.V = new double[CharSet.CharCount * (1 + (2 * xfmr.Radius)), xfmr.Size];
        }
    }
    internal class AttentionMem
    {
        public double[,] input;

        public double[,] Q;
        public double[,] K;
        public double[,] V;

        public double[,] scores;
        public double[,] weights;
        public double[,] attn;

        public double[] attention;
    }
}
