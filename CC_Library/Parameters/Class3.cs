using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Parameters
{
    public sealed class FireRating : Param
    {
        public string = "FireProtection";
        public string name = "FIRE RATING";
        public int type = ParamType.String;
        public Guid ID = Guid.NewGuid("4333b0f4-1652-4d7a-8060-2044707e0815");
        public Boolean Vis; = true;
        public string Description = "Fire Ratings foor Doors, Windows, and Walls.";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true;
    }
    public sealed class HardwareSet : Param
    {
        public string = "Doors";
        public string name = "HARDWARE SET";
        public int type = ParamType.String;
        public Guid ID = Guid.NewGuid("c58d15e9-068f-486c-886c-7e6db68b97e1");
        public Boolean Vis; = true;
        public string Description = "Hardware set of each type of door.";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true
    }
    class Class3
    {
        /*
        Finished Door Schedule
        
        Fire Rating
        HW Set
        
        DOOR SCHEDULE
        
        Door Number
        Door Access Control
        Type
        Opening Width
        Opening Height
        Door Thickness
        Door Material
        Door Finish
        Frame Material
        Frame Push
        Frame Pull
        Jamb Detail
        Head Detail
        Sill Detail
        Remarks
        
        WALL SCHEDULE
        Wall Type
        Stud Size
        Approximate Overall Dimension
        Fire Rating
        UL Number
        Insulation Type
        Notes
        */
    }
}
