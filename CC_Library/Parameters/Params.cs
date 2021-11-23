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
        //Examples : "Exit Stair 01" "Exit Door 02"
        public static readonly Param ExitName = new Param
            ("Exit Name",
            new Guid("812ad70f-30fd-4761-bd7c-017a72825150"),
            Subcategory.Egress,
            ParamType.Text,
            true,
            true );
        public static readonly Param ExitCapacity = new Param
            ("Exit Capacity",
            new Guid("f5486965-56e2-4f4b-85c0-55bc1d378cac"),
            Subcategory.Egress,
            ParamType.Int,
            false,
            true
            );
        public static readonly Param ExitLoad = new Param
            ("Exit Load",
            new Guid("08603c12-df1c-4481-8501-5e121a517764"),
            Subcategory.Egress,
            ParamType.Int,
            true,
            true );
        public static readonly Param ExitCapacityFactor = new Param
            ("Exit Capacity Factor",
            new Guid("891438ca-d346-47f7-b1a7-a0ccfd27ec91"),
            Subcategory.Egress,
            ParamType.Length,
            false,
            true );
        public static readonly Param EgressWidth = new Param
            ("Egress Width",
            new Guid("9acdeb5f-9b15-4af9-b7e0-a82a07f67127"),
            Subcategory.Egress,
            ParamType.Length,
            false,
            true );
        public static readonly Param RequiredEgressWidth = new Param
            ("Required Egress Width",
            new Guid("3785740b-26bf-4c73-81b6-8c7c75d25983"),
            Subcategory.Egress,
            ParamType.Length,
            true,
            true );
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
