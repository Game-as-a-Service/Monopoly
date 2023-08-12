using Microsoft.AspNetCore.Components;

namespace Client.Components;

public partial class Map : ComponentBase
{
    private int width = 0;
    private int height = 0;
    private readonly int _blockWidth = 133;
    private readonly int _blockHeight = 148;
    private Block[][] Data { get; set; } = null!;
    protected override Task OnInitializedAsync()
    {
        var emptyBlock = new EmptyBlock();
        var land = (string id, LandType landType) => new Land(id, landType);
        var road = (string id, RoadType roadType) => new Road(id, roadType);
        var stationLand = (string id, LandType landType) => new StationLand(id, landType);
        var stationRoad = (string id, StationRoadType stationType) => new StationRoad(id, stationType);
        Data = new Block[][]
        {
            new Block[] { emptyBlock, emptyBlock, land("A1", LandType.Down), land("A2", LandType.Down), stationLand("S1", LandType.Down), land("A3", LandType.Down), land("A4", LandType.Down), land("A5", LandType.Down), emptyBlock, emptyBlock, emptyBlock },
            new Block[] { emptyBlock, new Start(), road("A1", RoadType.row), road("A2", RoadType.row), stationRoad("S1", StationRoadType.row), road("A3", RoadType.row), road("A4", RoadType.row), road("A5", RoadType.corner_upper_right), emptyBlock, emptyBlock, emptyBlock },
            new Block[] { land("E4", LandType.Right), road("E4", RoadType.col), emptyBlock, emptyBlock, land("B1", LandType.Down), land("B2", LandType.Down), land("B3", LandType.Down), road(null, RoadType.col), land("B4", LandType.Down), land("B5", LandType.Down), emptyBlock },
            new Block[] { stationLand("S4", LandType.Right), stationRoad("S4", StationRoadType.col), land("E2", LandType.Down), land("E1", LandType.Down), road("B1", RoadType.corner_upper_left), road("B2", RoadType.row), road("B3", RoadType.row), new ParkingLot(), road("B4", RoadType.row), road("B5", RoadType.corner_upper_right), emptyBlock },
            new Block[] { land("E3", LandType.Right), road("E3", RoadType.corner_bottom_left), road("E2", RoadType.row), road("E1", RoadType.row), new Prison(), land("C3", LandType.Down), land("S3", LandType.Down), land("C2", LandType.Down), land("C1", LandType.Down), stationRoad("S2", StationRoadType.col), land("S2", LandType.Left) },
            new Block[] { emptyBlock, emptyBlock , emptyBlock , land("C4", LandType.Right) , road("C4", RoadType.corner_bottom_left) , road("C3", RoadType.row) , stationRoad("S3", StationRoadType.row), road("C2", RoadType.row), road("C1", RoadType.row) , road(null, RoadType.corner_bottom_right), emptyBlock }
        };

        // 從Value自動計算地圖大小 每個Block的大小為 133 * 148，間隔 1
        width = Data[0].Length * _blockWidth + Data[0].Length - 1;
        height = Data.Length * _blockHeight + Data.Length - 1;
        return base.OnInitializedAsync();
    }

    internal abstract record Block(string Id, string? Image = null, string? ImageStyle = null);

    internal record EmptyBlock() : Block("Empty Block");

    internal record Road(string Id, RoadType RoadType) : Block(Id, $"road_{RoadType}");

    internal enum RoadType
    {
        corner_upper_left, // ╔
                           //╦
        corner_upper_right, // ╗
                            //╠
                            //╬
                            //╣
        corner_bottom_left, // ╚
                            //╩
        corner_bottom_right, // ╝
        row, // ═
        col // ║
    }

    internal record Land : Block
    {
        public Land(string Id, LandType LandType) : base(Id)
        {
            this.Id = Id;
            this.LandType = LandType;
            switch (LandType)
            {
                case LandType.Down:
                    Image = "land_row";
                    ImageStyle = "padding-top: 84px";
                    break;
                case LandType.Left:
                    Image = "land_col";
                    ImageStyle = "padding-right: 37px";
                    break;
                case LandType.Right:
                    Image = "land_col";
                    ImageStyle = "padding-left: 37px";
                    break;
                case LandType.Up:
                    break;
                default:
                    break;
            }

        }

        public string Id { get; }
        public LandType LandType { get; }
    }
    internal enum LandType
    {
        Right,
        Down,
        Left,
        Up
    }

    internal record StationRoad(string Id, StationRoadType StationType) : Block(Id, $"railway_{StationType}");
    internal enum StationRoadType
    {
        row,
        col,
    }

    internal record StationLand(string Id, LandType LandType) : Land(Id, LandType);
    internal record Start() : Block("Start", "start");
    internal record ParkingLot() : Block("ParkingLot", "parking");
    internal record Prison() : Block("Prison", "prison");
}
