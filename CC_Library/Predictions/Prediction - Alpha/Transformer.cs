using System;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    [Serializable]
    public class Transformer
    {
        private const double Rate = 0.01;
        public string Name { get; set; }
        public int ValueSize {get;}
        public int QuerySize { get; }
        public int Inputs { get; }
        public int _Prefix { get; }
        public int _Suffix { get; }
        double[,] Queries { get; set; }
        double[,] Keys { get; set; }
        double[,] Values { get; set; }

        #region overloads
        public Transformer(string _Name, int _InputSize, int _ValueSize, int _QuerySize, int Prefix = 0, int Suffix = 0)
        {
            Name = _Name;
            ValueSize = _ValueSize;
            QuerySize = _QuerySize;
            Inputs = _InputSize;
            _Prefix = Prefix;
            _Suffix = Suffix;
            Queries = new double[_InputSize, QuerySize]; //new double[CharSet.CharCount * (1 + (2 * Radius)), QuerySize];
            Keys = new double[_InputSize, QuerySize]; //new double[CharSet.CharCount * (1 + (2 * Radius)), QuerySize];
            Values = new double[_InputSize, ValueSize - (Prefix + Suffix)]; //new double[CharSet.CharCount * (1 + (2 * Radius)), ValueSize];
            Queries.SetRandom();
            Keys.SetRandom();
            Values.SetRandom();
        }
        #endregion

        internal double[,] Forward(double[,] _input, AttentionMem mem)
        {
            //s.Locate(Radius)
            mem.input = _input; //Size should be s.Length, CharCount * Diameter

            try { mem.Q = mem.input.Dot(Queries); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size
            try { mem.K = mem.input.Dot(Keys); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size
            try { mem.V = mem.input.Dot(Values); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, size

            try { mem.scores = mem.Q.Dot(mem.K.Transpose()); }//Size should be s.Length, s.Length
            catch (Exception e) { e.OutputError(); }
            mem.scores = mem.scores.Divide(Math.Sqrt(QuerySize));
            try { mem.weights = Activations.SoftMax(mem.scores); } catch (Exception e) { e.OutputError(); } //Size should be s.Length, s.Length
            try
            {
                mem.attn = new double[_input.GetLength(0), ValueSize];
                var prepended = mem.weights.Dot(mem.V);
                Parallel.For(0, _input.GetLength(0), i =>
                {
                    Parallel.For(_Prefix, ValueSize - _Suffix, j =>
                    {
                        mem.attn[i, j] = prepended[i, j - _Prefix];
                    });
                });
            } //Size should be s.Length, size
            catch (Exception e) { e.OutputError(); }
            return mem.attn;

        }
        internal double[,] Backward(AttentionMem mem, AttentionChange change, double[,] dvals) //dvals Size is always size
        {
            var ndvals = new double[dvals.GetLength(0), dvals.GetLength(1) - (_Prefix + _Suffix)];
            Parallel.For(0, dvals.GetLength(0), i =>
            {
                Parallel.For(0, ValueSize - (_Prefix + _Suffix), j =>
                {
                    ndvals[i, j] = dvals[i, j + _Prefix];
                });
            });

            var DInputs = new double[mem.input.GetLength(0), mem.input.GetLength(1)];
            var Vdvals = mem.weights.Transpose().Dot(ndvals); //returns a vector [size, s.Length] - changed to [s.Length, size]
            var DV = mem.input.Transpose().Dot(Vdvals); //size is CharCount * diameter, size

            var dweights = mem.V.Dot(ndvals.Transpose()); // Size of this is s.Length, s.Length
            dweights = Activations.InverseSoftMax(dweights, mem.weights); // Size of this is s.Length, s.Length
            var Qdvals = dweights.Dot(mem.K); //size is s.Length, size
            var Kdvals = dweights.Dot(mem.Q); //size is s.Length, size
            var DQ = mem.input.Transpose().Dot(Qdvals); //this needs to be CharCount * diameter, size
            var DK = mem.input.Transpose().Dot(Kdvals);

            change.Q.Add(DQ);
            change.K.Add(DK);
            change.V.Add(DV);

            DInputs.Add(Vdvals.Dot(Values.Transpose()));
            DInputs.Add(Qdvals.Dot(Queries.Transpose()));
            DInputs.Add(Kdvals.Dot(Keys.Transpose()));

            return DInputs;
        }
        public double[,] Forward(double[,] input)
        {
            var output = new double[Inputs, ValueSize];

            try
            {
                var Q = input.Dot(Queries); //Size should be s.Length, size
                var K = input.Dot(Keys); //Size should be s.Length, size
                var V = input.Dot(Values); //Size should be s.Length, size

                var scores = Q.Dot(K.Transpose()); //Size should be s.Length, s.Length
                var weights = Activations.SoftMax(scores); //Size should be s.Length, s.Length
                var prepended = weights.Dot(V); //Size should be s.Length, size
                Parallel.For(0, input.GetLength(0), i =>
                {
                    Parallel.For(_Prefix, ValueSize - _Suffix, j =>
                    {
                        output[i, j] = prepended[i, j - _Prefix];
                    });
                });
            }
            catch (Exception e) { e.OutputError(); }
            return output;
        }
        internal void Update(AttentionChange change, WriteToCMDLine write)
        {
            Queries.Update(change.Q, Rate);
            Keys.Update(change.K, Rate);
            Values.Update(change.V, Rate);
        }
    }
    internal class AttentionChange
    {
        public double[,] Q;
        public double[,] K;
        public double[,] V;

        public AttentionChange(Transformer xfmr)
        {
            this.Q = new double[xfmr.Inputs, xfmr.QuerySize];
            this.K = new double[xfmr.Inputs, xfmr.QuerySize];
            this.V = new double[xfmr.Inputs, xfmr.ValueSize - (xfmr._Prefix + xfmr._Suffix)];
        }
    }
    public class AttentionMem
    {
        public double[,] input;

        public double[,] Q;
        public double[,] K;
        public double[,] V;

        public double[,] scores;
        public double[,] weights;
        public double[,] attn;
    }
}
