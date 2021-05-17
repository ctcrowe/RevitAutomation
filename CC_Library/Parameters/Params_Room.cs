using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Parameters
{
    /*
SpecificDemo = f6183af9c3f3438192003a86875dbb0d
InScope = a533a5849a7c48b6ac2ae37b9e2718ba
PanelFinish = b1faed092fed4be4ad760bde8d80320a
FrameFinish = ec32cfe55b2e41a9aa76c390643acd2b
WallThickness = fc75b64a7e4b40a380f34835f6ce09a4
EWallModified = e70e6e5b00c74f3c83ca7364d40f3ac8
FinishMaterial = b8b5c2b1c93c458c9ab5ed7237efdd85
MatDescription = f297a6cec3eb4922a421c466e915a53a
PatternStyle = a4ef2c1e6acb44539af197b21e79dba1
ColorFinish = e3ed247d44b944c1834ebe864f082581
Model = b206cea7ffe4400ca2cd314c6dea3874
Manufacturer = b7200c06c5ef4507874b1a49b7ef0043
Category = f1806391703247118e6978910e72a4d5
PredictedViewType = dfa22400e6574f0c81dce80f52e3753b
OccupantLoadFactor = f297d80f2354456bb3919725ab1213bb
OccupantLoad = d70e6cbbda3c4cc696ae8205d0fee131
WestWall = e47de3a34e594b15b2b5340adaf0839a
SouthWall = c46cb725a99f4ce2868a944d3c4bcd05
EastWall = adcdb1aa568e418fb2a41e31142d6382
NorthWall = ff7abba6189c43879a3976652a6c3cde
Base = d5bf6225a5664b0894c27da5962dabc5
FloorFinish = da1bf0f14a90474cb2da2a15e3a7c5fb
CeilingFinish = dbba2cbeb987404b9d268686b03c4a90
OccupancyClassification = e70bfcf9a76c4cde9397240320339ed9
RoomPrivacy = dc561de76d04440faceffacc4aaf7249
SpaceUserGroup = dbcd6e0bde4e4fa686fb49e200535498
RoomCategory = e7b892ffe8a14bdb9df273d1756f60db
FamUserGroup = ed7833f879f847eea714f03ae85bd716
CC_ID = e8bfeab8a49549238cbaa7a402f59d57
Masterformat = c05f36a248a749dcb549c9b96d885fd7
DetailDescription = d5b5270ddd5c44abb43d8e72b6709669
OverallWidth = d0249e62f16141868704e89c096cbbc9
OverallDepth = e055871799724976b4c330daadcc8020
OverallHeight = cfc88653c63b47efb165380d7e63600c
ULListing = bea192bd04e54374815266e7819fb26b
StudSize = dba7d515a4b34f55a374f5744367206f
Thickness = bd83390498b94237bd5273e758084c6a
Finish = a6c13e05bbb6482fa6a16e25fd3ae1af
HardwareGroup = c17ded9a28a24ac9a9b8b079877e9016
PanelType = d38f24810a834934b3ef86ee15d9cfe8
HeadDetail = c9ba846fd1f7459791daab1f109a88d6
JambDetail = bfe5dc8dfdad4e508f764c0352cacbd5
SillDetail = e82782e8c18140268b8c83ba4af92021
PanelThickness = ed9dcf40ab384c94a1c97337ff56e18f
FrameMaterial = e7bece7b3ce24dd7a26b069f3e60e9c6
PanelMaterial = ceaa6e3810dd4d23a061e6f11bd799d2
Glazing = f83ed1f92c1d48e6adf131c391ce8039
FireRating = a3abf061b101424683a9d6ba022db960
*/

    public static class RoomParams
    {
        private static Subcategory sub = Subcategory.Rooms;
        public static readonly Param SpaceUserGroup = new Param
            ("SpaceUserGroup",
            new Guid("dbcd6e0bde4e4fa686fb49e200535498"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param RoomCategory = new Param
            ("RoomCategory",
            new Guid("e7b892ffe8a14bdb9df273d1756f60db"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param OccupancyClassification = new Param
            ("OccupancyClassification",
            new Guid("e70bfcf9a76c4cde9397240320339ed9"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param CeilingFinish = new Param
            ("CeilingFinish",
            new Guid("dbba2cbeb987404b9d268686b03c4a90"),
            sub,
            ParamType.Text,
            true);
        public static readonly Param FloorFinish = new Param
            ("FloorFinish",
            new Guid("da1bf0f14a90474cb2da2a15e3a7c5fb"),
            sub,
            ParamType.Text,
            true,
            true);
        public static readonly Param Base = new Param
            ("Base",
            new Guid("d5bf6225a5664b0894c27da5962dabc5"),
            sub,
            ParamType.Text,
            true,
            true);
        public static readonly Param NorthWall = new Param
            ("NorthWall",
            new Guid("ff7abba6189c43879a3976652a6c3cde"),
            sub,
            ParamType.Text,
            true,
            true);
        public static readonly Param EastWall = new Param
            ("EastWall",
            new Guid("adcdb1aa568e418fb2a41e31142d6382"),
            sub,
            ParamType.Text,
            true,
            true);
        public static readonly Param SouthWall = new Param
            ("SouthWall",
            new Guid("c46cb725a99f4ce2868a944d3c4bcd05"),
            sub,
            ParamType.Text,
            true,
            true);
        public static readonly Param WestWall = new Param
            ("WestWall",
            new Guid("e47de3a34e594b15b2b5340adaf0839a"),
            sub,
            ParamType.Text,
            true,
            true);
        public static readonly Param OccupantLoad = new Param
            ("OccupantLoad",
            new Guid("d70e6cbbda3c4cc696ae8205d0fee131"),
            sub,
            ParamType.Int,
            true,
            true);
        public static readonly Param OccupantLoadFactor = new Param
            ("OccupantLoadFactor",
            new Guid("f297d80f2354456bb3919725ab1213bb"),
            sub,
            ParamType.Int,
            true,
            true);
        public static readonly Param RoomPrivacy = new Param
            ("RoomPrivacy",
            new Guid("dc561de76d04440faceffacc4aaf7249"),
            sub,
            ParamType.Text,
            true,
            true);
    }
}
