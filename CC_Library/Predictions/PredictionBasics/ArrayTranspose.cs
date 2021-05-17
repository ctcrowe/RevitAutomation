namespace CC_Library.Predictions
{
    internal static class Array_Transpose
    {
        public static double[,] Transpose(this double[,] Array)
        {
            int x = Array.GetLength(1);
            int y = Array.GetLength(0);
            double[,] result = new double[x, y];
            for(int i = 0; i < x; i++)
                for(int j = 0; j < y; j++)
                    result[i, j] = Array[j, i];
            return result;
        }
    }
}