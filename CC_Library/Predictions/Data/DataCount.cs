using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class DataCount
    {
        public static int Count(this Datatype datatype)
        {
            int value = (int)datatype / 1000;
            int fin = value % 10;
            return fin;
        }
    }
}
