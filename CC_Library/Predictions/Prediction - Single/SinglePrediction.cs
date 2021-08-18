
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class PredictionBasis
    {
        public Datatype datatype;
        public NeuralNetwork network {get; set;}
        public int TextCount {get; set;}
    }
    public static class SinglePrediction
    {
        public static double[] Predict(this PredictionBasis basis, Sample s)
        {
            Alpha a = new Alpha(new WriteToCMDLine(WriteNull));
            if(basis.TextCount > 0)
                AlphaContext ctxt1 = new AlphaContext(basis.Datatype, new WriteToCMDLine(WriteNull));
            if(basis.TextCount > 1)
                AlphaContext ctxt2 = new AlphaContext(basis.Datatype, new WriteToCMDLine(WriteNull));
        }
    }
}
