namespace CC_Library.Datatypes
{
    /*
     * Datatype number will reference the following
     * 1000s) references the number of values in the array representing the feature in space.
     *          An n dimensional array will represent it.
     *          Each value of this array does NOT have to be linear when compared against another
     *              EX: 1st might reference dimension 3, 2nd dimension 4, etc.
     *          Every array will be smaller than the Dictionary Array, which will contain 7 numbers.
     *          This allows relationships to develop between Datatypes that are not exclusively interrelated.
     *          Some datatypes will have stronger relations to each other than others.
     */
    public enum Datatype
    {
        Null = 0,
        OccupantLoadFactor = 3001,
        StudLayer = 3002,
        OccupancyGroup = 3003,
        RequiredFamilies = 5001,
        Subcategory = 5002,
        RoomPrivacy = 5003,
        Keynote = 5004,
        Masterformat = 6001,
        Dictionary = 8001
    }
}