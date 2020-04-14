using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal class Dataset
    {
        public Datatype datatype { get; }
        public List<DataPoint> Data { get; }
        public Dataset(Datatype dtype)
        {
            this.datatype = dtype;
            this.Data = new List<DataPoint>();
        }
    }
}