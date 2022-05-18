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
    internal class XfmrSet
    {
        public Transformer AlphaXfmr1 = new Transformer("AlphaXfmr1", 400);
    }
}
