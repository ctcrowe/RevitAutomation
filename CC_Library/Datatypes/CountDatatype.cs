namespace CC_Library.Datatypes
{
    public static class DataCount
    {
        public static int Count(this Datatype datatype)
        {
            int value = (int)datatype / 1000;
            int fin = value % 10;
            return fin;
        }
    }
}