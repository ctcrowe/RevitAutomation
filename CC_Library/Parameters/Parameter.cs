namespace CC_Library.Parameters
{
    //0 is the Universal ID Parameter
    //Negative Parameters are Instances
    //Parameters ending in 0 are not user modifiable
    //Parameters in the 4000 range are View Parameters
    //Parameters in the 3000 range are room parameters
    //Parameters in the 2000 range are family only
    //Parameters in the 1000 range are Project Information Parameters
    //Parameters Absolute Value below 100 are strings
    //Parameters from 100-199 are length type parameters
    //Parameters from 200-299 are yes / no type parameters
    public enum CCParameter
    {
        PredictedViewType = -4000,
        RoomPrivacy = -3101,
        InScope = -3201,
        SpaceUserGroup = -3002,
        RoomCategory = -3001,
        FamUserGroup = -2001,
        CC_ID = 0,
        Masterformat = 2000,
        DetailDescription = 2002,
        OverallWidth = 2101,
        OverallDepth = 2102,
        OverallHeight = 2103
    }
}