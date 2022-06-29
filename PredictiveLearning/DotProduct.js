function Dot(x, y) {
  if(!Array.isArray(x)) { return "X is not an Array"; }
  if(!Array.isArray(y)) { return "Y is not an Array"; }

    if(Array.isArray(x[0])) {
      if(Array.isArray(y[0])) {
        const result = new Array(x.length);
        for(let ylen = 0; ylen < result.length; ylen++) {
          result[ylen] = new Array(y[0].length);
          for(let ytot = 0; ytot < result[ylen].length; ytot++) {
            result[ylen][ytot] = 0;
          }
        }

        for(let i = 0; i < x.length; i++) {
          for(let j = 0; j < y.length; j++) {
            for(let k = 0; k < y[0].length; k++) {
              result[i][k] += x[i][j] * y[j][k];
            }
          }
        }
        return result;
      }
      else {
        return "Y is not a 2D Array";
      }
    }
    else {
      if(Array.isArray(y[0])) {
        return "X is not a 2D Array";
      }
      return "X and Y are not 2D Arrays";
    }
}
