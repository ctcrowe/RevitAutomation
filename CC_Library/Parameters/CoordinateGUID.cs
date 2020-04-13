using System;

namespace CC_Library.Parameters
{
    public static class CoordinateGUIDs
    {
        public static Guid GetGUID(this CCParameter par)
        {
            int j = (int) par;
            ParamGUID pg;
            if(Enum.TryParse(j.ToString(), out pg))
            {
                return new Guid(pg.ToString());
            }
            return Guid.NewGuid();
        }
    }
}
