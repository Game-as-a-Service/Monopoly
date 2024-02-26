using SharedLibrary.MonopolyMap;
using System.ComponentModel;

namespace Client.Components;

public class BlazorMap
{
    private readonly int totalWidth;
    private readonly int totalHeight;
    private readonly int _blockWidth = 133;
    private readonly int _blockHeight = 148;
    private readonly int _blockMargin = 5;
    private readonly Block[][] _blocks;

    public int TotalWidth => totalWidth;

    public int TotalHeight => totalHeight;

    public Block[][] Blocks => _blocks;

    public int BlockWidth => _blockWidth;

    public int BlockHeight => _blockHeight;

    public BlazorMap(MonopolyMap data)
    {
        // 自動Mapping _data 到 _blocks
        _blocks = new Block[data.Data.Length][];
        for (int i = 0; i < data.Data.Length; i++)
        {
            _blocks[i] = new Block[data.Data[i].Length];
            for (int j = 0; j < data.Data[i].Length; j++)
            {
                var monopolyBlock = data.Data[i][j];
                var x = j * (_blockWidth + _blockMargin);
                var y = i * (_blockHeight + _blockMargin);
                _blocks[i][j] = monopolyBlock.Type switch
                {
                    BlockType.None => null!,
                    BlockType.Land => CreateLand(monopolyBlock, x, y),
                    BlockType.Road => new Road(x, y, monopolyBlock.Id, monopolyBlock.ToRoad().RoadType.ToBlazorRoadType()),
                    BlockType.ParkingLot => new ParkingLot(x, y, monopolyBlock.Id),
                    BlockType.Prison => new Prison(x, y, monopolyBlock.Id),
                    BlockType.StartPoint => new StartPoint(x, y, monopolyBlock.Id),
                    BlockType.Station => new Station(x, y, monopolyBlock.Id, monopolyBlock.ToStation().RoadType.ToBlazorStationRoadType()),
                    _ => throw new InvalidEnumArgumentException()
                };
            }
        }

        totalWidth = data.Data[0].Length * (_blockWidth + _blockMargin) - _blockMargin;
        totalHeight = data.Data.Length * (_blockHeight + _blockMargin) - _blockMargin;
    }

    /// <summary>
    /// 根據不同方向的土地，修改座標後傳回
    /// </summary>
    /// <param name="block"></param>
    /// <param name="_x">原座標X</param>
    /// <param name="_y">原座標Y</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Land CreateLand(BlockBase block, int x, int y)
    {
        var type = block.ToLand().LandType.ToBlazorLandType();
        double offsetX = type switch
        {
            LandType.Up => 0,
            LandType.Down => 0,
            LandType.Right => 13.08,
            LandType.Left => -13.08,
            _ => throw new InvalidOperationException()
        };
        double offsetY = type switch
        {
            LandType.Up => -42,
            LandType.Down => 42,
            LandType.Right => 0,
            LandType.Left => 0,
            _ => throw new InvalidOperationException()
        };
        return new Land(x, y, block.Id, type, offsetX, offsetY);
    }

    public abstract record Block(int X, int Y, string Id, string ImageName, double OffsetX = 0, double OffsetY = 0);
    private record Road(int X, int Y, string Id, RoadType RoadType) : Block(X, Y, Id, RoadType.GetImageName());
    private record Land(int X, int Y, string Id, LandType LandType, double OffsetX, double OffsetY) : Block(X, Y, Id, LandType.GetImageName(), OffsetX, OffsetY);
    private record ParkingLot(int X, int Y, string Id) : Block(X, Y, Id, "parkinglot");
    private record Prison(int X, int Y, string Id) : Block(X, Y, Id, "prison");
    private record StartPoint(int X, int Y, string Id) : Block(X, Y, Id, "startpoint");
    private record Station(int X, int Y, string Id, StationRoadType StationRoadType) : Block(X, Y, Id, StationRoadType.GetImageName());

    public enum RoadType
    {
        [ImageName("road_top_left")]
        TopLeftIntersection,      // ╔
        [ImageName("road_top")]
        TopIntersection,          // ╦
        [ImageName("road_top_right")]
        TopRightIntersection,     // ╗
        [ImageName("road_left_center")]
        LeftCenterIntersection,   // ╠
        [ImageName("road_center")]
        CenterIntersection,       // ╬
        [ImageName("road_right_center")]
        RightCenterIntersection,  // ╣
        [ImageName("road_bottom_left")]
        BottomLeftIntersection,   // ╚
        [ImageName("road_bottom")]
        BottomIntersection,       // ╩
        [ImageName("road_bottom_right")]
        BottomRightIntersection,  // ╝
        [ImageName("road_horizontal")]
        HorizontalRoad,           // ═
        [ImageName("road_vertical")]
        VerticalRoad              // ║
    }

    public enum StationRoadType
    {
        [ImageName("railway_top_left")]
        TopLeftIntersection,      // ╔
        [ImageName("railway_top")]
        TopIntersection,          // ╦
        [ImageName("railway_top_right")]
        TopRightIntersection,     // ╗
        [ImageName("railway_left_center")]
        LeftCenterIntersection,   // ╠
        [ImageName("railway_center")]
        CenterIntersection,       // ╬
        [ImageName("railway_right_center")]
        RightCenterIntersection,  // ╣
        [ImageName("railway_bottom_left")]
        BottomLeftIntersection,   // ╚
        [ImageName("railway_bottom")]
        BottomIntersection,       // ╩
        [ImageName("railway_bottom_right")]
        BottomRightIntersection,  // ╝
        [ImageName("railway_horizontal")]
        HorizontalRoad,           // ═
        [ImageName("railway_vertical")]
        VerticalRoad              // ║
    }

    public enum LandType
    {
        [ImageName("land_vertical")]
        Right,
        [ImageName("land_horizontal")]
        Down,
        [ImageName("land_vertical")]
        Left,
        [ImageName("land_horizontal")]
        Up
    }
}

static class BlazorMapExtensions
{
    private static string GetImageName(Type type, Enum enumValue)
    {
        var memInfo = type.GetMember(enumValue.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(ImageNameAttribute), false);
        return ((ImageNameAttribute)attributes[0]).ImageName;
    }

    public static string GetImageName(this BlazorMap.RoadType roadType)
    {
        return GetImageName(typeof(BlazorMap.RoadType), roadType);
    }

    public static string GetImageName(this BlazorMap.LandType landType)
    {
        return GetImageName(typeof(BlazorMap.LandType), landType);
    }

    public static string GetImageName(this BlazorMap.StationRoadType stationRoadType)
    {
        return GetImageName(typeof(BlazorMap.StationRoadType), stationRoadType);
    }

    public static Land ToLand(this BlockBase block)
    {
        return (Land)block;
    }

    public static BlazorMap.LandType ToBlazorLandType(this LandType landType)
    {
        return landType switch
        {
            LandType.Right => BlazorMap.LandType.Right,
            LandType.Down => BlazorMap.LandType.Down,
            LandType.Left => BlazorMap.LandType.Left,
            LandType.Up => BlazorMap.LandType.Up,
            _ => throw new InvalidEnumArgumentException()
        };
    }

    public static Road ToRoad(this BlockBase block)
    {
        return (Road)block;
    }

    public static BlazorMap.RoadType ToBlazorRoadType(this RoadType roadType)
    {
        return roadType switch
        {
            RoadType.TopLeftIntersection => BlazorMap.RoadType.TopLeftIntersection,
            RoadType.TopIntersection => BlazorMap.RoadType.TopIntersection,
            RoadType.TopRightIntersection => BlazorMap.RoadType.TopRightIntersection,
            RoadType.LeftCenterIntersection => BlazorMap.RoadType.LeftCenterIntersection,
            RoadType.CenterIntersection => BlazorMap.RoadType.CenterIntersection,
            RoadType.RightCenterIntersection => BlazorMap.RoadType.RightCenterIntersection,
            RoadType.BottomLeftIntersection => BlazorMap.RoadType.BottomLeftIntersection,
            RoadType.BottomIntersection => BlazorMap.RoadType.BottomIntersection,
            RoadType.BottomRightIntersection => BlazorMap.RoadType.BottomRightIntersection,
            RoadType.HorizontalRoad => BlazorMap.RoadType.HorizontalRoad,
            RoadType.VerticalRoad => BlazorMap.RoadType.VerticalRoad,
            _ => throw new InvalidEnumArgumentException()
        };
    }

    public static Station ToStation(this BlockBase block)
    {
        return (Station)block;
    }

    public static BlazorMap.StationRoadType ToBlazorStationRoadType(this RoadType stationRoadType)
    {
        return stationRoadType switch
        {
            RoadType.TopLeftIntersection => BlazorMap.StationRoadType.TopLeftIntersection,
            RoadType.TopIntersection => BlazorMap.StationRoadType.TopIntersection,
            RoadType.TopRightIntersection => BlazorMap.StationRoadType.TopRightIntersection,
            RoadType.LeftCenterIntersection => BlazorMap.StationRoadType.LeftCenterIntersection,
            RoadType.CenterIntersection => BlazorMap.StationRoadType.CenterIntersection,
            RoadType.RightCenterIntersection => BlazorMap.StationRoadType.RightCenterIntersection,
            RoadType.BottomLeftIntersection => BlazorMap.StationRoadType.BottomLeftIntersection,
            RoadType.BottomIntersection => BlazorMap.StationRoadType.BottomIntersection,
            RoadType.BottomRightIntersection => BlazorMap.StationRoadType.BottomRightIntersection,
            RoadType.HorizontalRoad => BlazorMap.StationRoadType.HorizontalRoad,
            RoadType.VerticalRoad => BlazorMap.StationRoadType.VerticalRoad,
            _ => throw new InvalidEnumArgumentException()
        };
    }
}

[AttributeUsage(AttributeTargets.Field)]
class ImageNameAttribute : Attribute
{
    public string ImageName { get; set; }
    public ImageNameAttribute(string imageName)
    {
        ImageName = imageName;
    }
}
