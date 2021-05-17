using System;

namespace CC_Library.Parameters
{
    public class Param : IEquatable<Param>
    {
        public Guid Guid { get; }
        public string Name { get; }
        public ParamType Type { get; }
        public Subcategory Subcategory { get; }
        public bool Instance { get; }
        public bool UserModifiable { get; }

        public Param(string name, Guid guid, Subcategory subcategory, ParamType type, bool instance)
        {
            this.Name = name;
            this.Guid = guid;
            this.Type = type;
            this.Subcategory = subcategory;
            this.Instance = instance;
            this.UserModifiable = true;
        }
        public Param(string name, Guid guid, Subcategory subcategory, ParamType type, bool instance, bool usermodifiable)
        {
            this.Name = name;
            this.Guid = guid;
            this.Type = type;
            this.Subcategory = subcategory;
            this.Instance = instance;
            this.UserModifiable = usermodifiable;
        }
        #region Equatable
        public bool Equals(Param other)
        {
            if (other == null)
                return false;
            if (other.Guid.GetHashCode() == this.Guid.GetHashCode())
                return true;
            return false;
        }
        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Param ParamObj = obj as Param;
            if (ParamObj == null)
                return false;
            else
                return Equals(ParamObj);
        }
        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }
        public static bool operator ==(Param p1, Param p2)
        {
            if (((object)p1) == null || ((object)p2) == null)
                return Object.Equals(p1, p2);

            return p1.Equals(p2);
        }
        public static bool operator !=(Param p1, Param p2)
        {
            if (((object)p1) == null || ((object)p2) == null)
                return ! Object.Equals(p1, p2);

            return ! p1.Equals(p2);
        }
        #endregion
    }
}