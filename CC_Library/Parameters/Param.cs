using System;

namespace CC_Library.Parameters
{
    public abstract class Param
    {
        public string ParamGroup;
        public string name;
        public int type;
        public Guid ID;
        public Boolean Vis;
        public string Description;
        public Boolean UsrMod;
        public Boolean Inst;
        public Boolean Fixed;
    }
}
