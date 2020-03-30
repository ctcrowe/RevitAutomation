namespace CC_Library.Parameters
{
    //0 is the Universal ID Parameter
    //Negative Parameters are Instances
    //Parameters in the 3000 range are room parameters
    //Parameters in the 2000 range are family only
    //Parameters in the 1000 range are Project Information Parameters
    //Parameters Absolute Value below 100 are strings
    //Parameters from 101-200 are length type parameters
    public enum CCParameter
    {
        CC_ID = 0,
        Masterformat = 2001,
        OverallWidth = 2101,
        OverallDepth = 2102,
        OverallHeight = 2103,
        RoomCategory = -3001,
        RoomPrivacy = -3101
    }
}