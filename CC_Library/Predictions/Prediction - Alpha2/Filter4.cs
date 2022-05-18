using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace CC_Library.Predictions
{
    [Serializable]
    internal class Transformer1 : IAlphaTransformer
    {
        public string Name { get { return "Transformer1"; } }
        public int Size { get { return 400; } }
        public const int Radius = 1;
        private const double rate = 0.1;

        public double[,] Queries { get; set; }
        public double[,] Keys { get; set; }
        public double[,] Values { get; set; }
        public Transformer1()
        {
            this.Queries = new double[CharSet.CharCount * (1 + (2 * Radius)), Size];
            this.Queries.SetRandom();
            this.Keys = new double[CharSet.CharCount * (1 + (2 * Radius)), Size];
            this.Keys.SetRandom();
            this.Values = new double[CharSet.CharCount * (1 + (2 * Radius)), Size];
            this.Values.SetRandom();
        }
        public double[] Forward(string s)
        {
            var input = s.Locate(Radius); //Size should be s.Length, CharCount * Diameter
            var output = new double[Size];
            
            try
            {
                var Q = input.Dot(Queries); //Size should be s.Length, size
                var K = input.Dot(Keys); //Size should be s.Length, size
                var V = input.Dot(Values); //Size should be s.Length, size

                var scores = Q.Dot(K.Transpose()); //Size should be s.Length, s.Length
                var weights = Activations.SoftMax( scores); //Size should be s.Length, s.Length
                var attn = weights.Dot(V);  //Size should be s.Length, size
                output = attn.SumRange();
            }
            catch (Exception e) { e.OutputError(); }
            return output;
        }
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

            try { mem.attention = mem.attn.SumRange();} catch (Exception e) { e.OutputError(); } //Size should be size
        }
        public void Backward(AttentionMem mem, AttentionChange change, double[] dvals) //dvals Size is always size
        {
            var atndvals = dvals.Dot(mem.attn.Ones()); //returns a vector [s.Length, size]
            var Vdvals = atndvals.Transpose().Dot(mem.weights); //returns a vector [size, s.Length]
            var DV = Vdvals.Dot(mem.input).Transpose(); //size is CharCount * diameter, size

            var dweights = atndvals.Dot(mem.V.Transpose()); // Size of this is s.Length, s.Length
            dweights = Activations.InverseSoftMax(dweights, mem.weights); // Size of this is s.Length, s.Length
            var Qdvals = dweights.Dot(mem.K); //size is s.Length, size
            var Kdvals = dweights.Transpose().Dot(mem.Q); //size is s.Length, size
            var DQ = mem.input.Transpose().Dot(Qdvals); //this needs to be CharCount * diameter, size
            var DK = mem.input.Transpose().Dot(Kdvals);

            change.Q.Add(DQ); 
            change.K.Add(DK);
            change.V.Add(DV);
        }
        public void Update(AttentionChange change, int Count, WriteToCMDLine write)
        {
            Queries.Update(change.Q, -1 * rate / Count);
            Keys.Update(change.K, -1 * rate / Count);
            Values.Update(change.V, -1 * rate / Count);
        }
    }
    internal class AttentionChange
    {
        public double[,] Q;
        public double[,] K;
        public double[,] V;

        public AttentionChange(IAlphaTransformer xfmr)
        {
            this.Q = new double[CharSet.CharCount * 3, xfmr.Size];
            this.K = new double[CharSet.CharCount * 3, xfmr.Size];
            this.V = new double[CharSet.CharCount * 3, xfmr.Size];
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
