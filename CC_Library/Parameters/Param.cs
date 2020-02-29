using System;

namespace CC_Library.Parameters
{
    public abstract class Param
    {
        public readonly string ParamGroup;
        public readonly string name;
        public readonly int type;
        public readonly Guid ID;
        public readonly Boolean Vis;
        public readonly string Description;
        public readonly Boolean UsrMod;
        public readonly Boolean Inst;
        public readonly Boolean Fixed;
        pubic string Value;
    }
}
