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

        /// <summary>
        /// New Datatypes:
        /// 7 Number Format
        /// Second Number Represents Number of Layers
        /// Third Number Represents Layer Size
        /// Fourth and Fifth Numbers represent the last layer size (Accept in the Dictionary)
        /// Last Two Numbers are to give it a unique ID
        /// All Entry and Exit information will be contained within 8 values

        /// A new datatype is created! Softmax Functions will begin with a "1"
        /// Other datatypes will begin with a 2
        /// if the system is a softmax system, the last layer needs to be the size of the options.

    public enum Datatype
    {
        Dictionary,
        Alpha,
        AlphaContext,
        AlphaLocation,
        GlobalContext,
        Beta,
        Masterformat,
        Div8Type,
        OccupantLoadFactor,
        StudLayer,
        OccupancyGroup,
        ObjectCategory,
        RequiredFamilies,
        Subcategory,
        RoomPrivacy,
        Keynote,
        Boundary,
        Elevation,
        Pointer
    }
}