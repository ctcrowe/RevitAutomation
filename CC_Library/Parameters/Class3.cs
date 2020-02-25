using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Parameters
{
    public sealed class FireRatingParam : Param
    {
        public string = "FireProtection";
        public string name = "FIRE RATING";
        public int type = ParamType.Double;
        public Guid ID = Guid.NewGuid("4333b0f4-1652-4d7a-8060-2044707e0815");
        public Boolean Vis; = true;
        public string Description = "Fire Ratings foor Doors, Windows, and Walls.";
        public Boolean UsrMod = true;
        public Boolean Inst = false;
        public Boolean Fixed = true
    }
    class Class3
    {
        /*
        DOOR SCHEDULE
        
        Door Number
        Fire Rating
        HW Set
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
