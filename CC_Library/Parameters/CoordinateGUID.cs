using System;

namespace CC_Library.Parameters
{
    public static class CoordinateGUIDs
    {
        public static Guid GetGUID(this CCParameter par)
        {
            int i = 0;
            foreach(CCParameter p in Enum.GetValues(typeof(CCParameter)))
            {
                if(p == par)
                {
                    ParamGUID pg = (ParamGUID)i;

                    return new Guid(pg.ToString());
                }
                i++;
            }
            return Guid.NewGuid();
        }
    }
}
