using System;

namespace CC_Library.Parameters
{
    public static class Params
    {
        public static readonly Param Masterformat = new Param
            ("MF Division",
            new Guid("aedbf7d2d73c44048cf7ed26f3231d25"),
            Subcategory.Generic,
            ParamType.Int,
            false,
            true);
        /*
        Add the below unfinished Params to Doors, Generic Models, and Stairs.
        */
        public static readonly Param ExitName = new Param
            (
            //Examples : "Exit Stair 01" "Exit Door 02"
            );
        public static readonly Param ExitCapacity = new Param
            (
            );
        public static readonly Param ExitLoad = new Param
            (
            );
        public static readonly Param ExitCapacityFactor = new Param
            (
            );
        public static readonly Param EgressWidth = new Param
            (
            );
        public static readonly Param RequiredEgressWidth = new Param
            (
            );
        public static readonly Param Finish = new Param
            ("Finish Material",
            new Guid("4848e7f4ee234755b9892deb5eef17b6"),
            Subcategory.Generic,
            ParamType.Material,
            false,
            true );
        public static readonly Param AreaPerOccupant = new Param
            ("Area Per Occupant",
             new Guid("793a29471a7c4255964dc616c0b785d6"),
             Subcategory.Rooms,
             ParamType.Area,
             true,
             true );
        public static readonly Param OccupancyGroup = new Param
            ("Occupancy Group",
             new Guid("733534473e214520bcdafc6093c787b1"),
             Subcategory.Rooms,
             ParamType.Text,
             true,
             true );
    }
}
