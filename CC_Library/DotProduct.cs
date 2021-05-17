using System.Threading.Tasks;


namespace CC_Library.Predictions
{
    internal static class DotProduct
    {
        public static double[,] Dot(this double[,] x, double[,] y)
        {
            if (x.GetLength(0) == y.GetLength(1) && x.GetLength(1) == y.GetLength(0))
            {
                double[,] dot = new double[x.GetLength(1), y.GetLength(0)];

                Parallel.For(0, x.GetLength(0), i =>
                {
                    Parallel.For(0, y.GetLength(1), j =>
                    {
                        Parallel.For(0, y.GetLength(0), k => dot[i, j] += x[i, k] * y[k, j]);
                    });
                });
                return dot;
            }
            return null;
        }
        public static double[] Dot(this double[,] x, double[] y)
        {
            if(x.GetLength(1) == y.GetLength(0))
            {
                double[] z = new double[x.GetLength(0)];
                for(int i = 0; i < x.GetLength(0); i++)
                {
                    for(int j = 0; j < x.GetLength(1); j++)
                    {
                        z[i] += x[i, j] * y[j];
                    }
                }
                return z;
            }
            return null;
        }
        public static double[] DotSwitch(this double[] x, double[] y)
        {
            if(x.GetLength(0) == y.GetLength(0))
            {
                double[] dot = new double[x.GetLength(0)];
                Parallel.For(0, x.GetLength(0), i =>
                {
                    dot[i] = x[i] * y[i];
                });
                return dot;
            }
            return null;
        }
    }
}
