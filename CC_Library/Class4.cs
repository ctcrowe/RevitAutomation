using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace CC_Plugin
{
    public class Param
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public string ParamGroup { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public Guid ID { get; set; }
        public Boolean Vis { get; set; }
        public string Description { get; set; }
        public Boolean UsrMod { get; set; }
        public Boolean Inst { get; set; }
        public Boolean Fixed { get; set; }
        public Param(string name, int type, Guid id, string paramgroup,
                     string description, bool vis, bool inst, bool usrmod, bool fix)
        {
            this.name = name;
            this.type = type;
            this.ID = id;
            this.ParamGroup = paramgroup;
            this.Description = description;
            this.Vis = vis;
            this.Inst = inst;
            this.UsrMod = usrmod;
            this.Fixed = fix;
        }
    }
    public static class ParamType
    {
        public const int Bool = 0;
        public const int Int = 1;
        public const int Double = 2;
        public const int Length = 3;
        public const int String = 4;
        public const int Material = 5;
        public const int Area = 6;
    }
}
