namespace CC_Library.Parameters
{
    //0 is the Universal ID Parameter
    //Negative Parameters are Instances
    //Parameters ending in 0 are not user modifiable
    //Parameteres in the 9000 range apply to Everything
    //Parameters in the 8000 range apply to Floors, Ceilings, and Rooms
    //Parameters in the 7000 range apply to Door and Window Families Only
    //Parameters in the 6000 range apply to Walls, Floors, and Ceilings
    //Parameters in the 4000 range are View Parameters
    //Parameters in the 3000 range are room parameters
    //Parameters in the 2000 range are family only
    //Parameters in the 1000 range are Project Information Parameters
    //Parameters Absolute Value below 100 are strings
    //Parameters from 100-199 are length type parameters
    //Parameters from 200-299 are yes / no type parameters
    //Parameters from 300-399 are integers
    //Parameters from 400-499 are Materials
    //Parameters from 500-599 are Areas
    public enum CCParameter
    {
        SpecificDemo = -9201,
        InScope = -8201,
        PanelFinish = -7402,
        FrameFinish = -7401,
        WallThickness = -7101,
        EWallModified = -6201,
        PredictedViewType = -4000,
        OccupantLoadFactor = -3501,
        OccupantLoad = -3301,
        WestWall = -3011,
        SouthWall = -3010,
        EastWall = -3009,
        NorthWall = -3008,
        Base = -3007,
        FloorFinish = -3006,
        CeilingFinish = -3005,
        OccupancyClassification = -3004,
        RoomPrivacy = -3003,
        SpaceUserGroup = -3002,
        RoomCategory = -3001,
        FamUserGroup = -2001,
        CC_ID = 0,
        Masterformat = 2000,
        DetailDescription = 2002,
        OverallWidth = 2101,
        OverallDepth = 2102,
        OverallHeight = 2103,
        FireRating = 6001,
        ULListing = 6002,
        StudSize = 6101,
        Thickness = 6102,
        Finish = 6201,
        HardwareGroup = 7001,
        FrameMaterial = 7401,
        PanelMaterial = 7402,
        PanelType = 7006,
        HeadDetail = 7007,
        JambDetail = 7008,
        SillDetail = 7009
    }
}