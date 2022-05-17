using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library;
using CC_Library.Datatypes;

        //[][][][][x] => value set
        //[][][][x] => dropout / no. 2 wide always
        //[][][x] => layer number - based on Filter Size (# of layers)
        //[][x] => location location location - this version will make a list and then convert it to an array.
        //[x] => layer group - 3 wide always - 0 = locations, 1 = context, 2 = combined output

        //[2][1][1][3][x] =>
        //  x = [0] = locations, ,[1] = locations, [2] = const int Size above

namespace CC_Library.Predictions
{
}
