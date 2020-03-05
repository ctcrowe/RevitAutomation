using System;

namespace CC_Library.Parameters
{
    public class Param
    {
        public string Group;
        public string Name;
        public int Type;
        public Guid ID;
        public Boolean IsVisible;
        public string Description;
        public Boolean IsUserModifiable;
        public Boolean IsInstance;
        public Boolean IsFixed;
        public ParamLocation Location;
        public string Value;

        public delegate void AddParam(dynamic doc);

        public Param(
            string pg,
            string n,
            int t,
            Guid id,
            Boolean v,
            string desc,
            Boolean u,
            Boolean i,
            Boolean f,
            ParamLocation L)
        {
            this.Group = pg;
            this.Name = n;
            this.Type = t;
            this.ID = id;
            this.IsVisible = v;
            this.Description = desc;
            this.IsUserModifiable = u;
            this.IsInstance = i;
            this.IsFixed = f;
            this.Location = L;
        }
    }
}
