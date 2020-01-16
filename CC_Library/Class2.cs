using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library
{
    public class Prediction
    {
        public string Word { get; }
        public int[] Predictions { get; set; }
        public int[] Previous { get; set; }

        public Prediction(string s)
        {
            this.Word = s;
            this.Predictions = new int[26];
            this.Previous = new int[26];
        }
    }
}
