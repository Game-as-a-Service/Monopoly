using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrary.MonopolyMap;

public class MonopolyMap
{
    public MonopolyMap(string Id, BlockBase[][] Data)
    {
        this.Id = Id;
        this.Data = Data;
    }

    public string Id { get; }
    public BlockBase[][] Data { get; }

    public static MonopolyMap Parse(string data)
    {
        return JsonSerializer.Deserialize<MonopolyMap>(data, JsonSerializerOptions);
    }
    public static JsonSerializerOptions JsonSerializerOptions => new()
    {
        WriteIndented = true,
        Converters = { new BlockConverter() }
    };
}
public abstract record BlockBase(string Id, BlockType Type);
public record EmptyBlock() : BlockBase("", BlockType.None);
public record Road(string Id, RoadType RoadType) : BlockBase(Id, BlockType.Road);
public record Land(string Id, LandType LandType) : BlockBase(Id, BlockType.Land);
public record ParkingLot(string Id) : BlockBase(Id, BlockType.ParkingLot);
public record Prison(string Id) : BlockBase(Id, BlockType.Prison);
public record StartPoint(string Id) : BlockBase(Id, BlockType.StartPoint);
public record Station(string Id, RoadType RoadType) : BlockBase(Id, BlockType.Station);
public enum BlockType
{
    None,
    Land,
    Road,
    Prison,
    ParkingLot,
    StartPoint,
    Station
}

public enum RoadType
{
    TopLeftIntersection,      // ╔
    TopIntersection,          // ╦
    TopRightIntersection,     // ╗
    LeftCenterIntersection,   // ╠
    CenterIntersection,       // ╬
    RightCenterIntersection,  // ╣
    BottomLeftIntersection,   // ╚
    BottomIntersection,       // ╩
    BottomRightIntersection,  // ╝
    HorizontalRoad,           // ═
    VerticalRoad              // ║
}

public enum LandType
{
    Right,
    Down,
    Left,
    Up
}

public class TestMap
{
    public MonopolyMap Map { get; set; }
    public TestMap()
    {
        var emptyBlock = new EmptyBlock();
        var land = (string id, LandType landType) => new Land(id, landType);
        var road = (string id, RoadType roadType) => new Road(id, roadType);
        var stationLand = (string id, LandType landType) => new Land(id, landType);
        var stationRoad = (string id, RoadType stationType) => new Station(id, stationType);
        Map = new MonopolyMap("test", new BlockBase[][]
            {
                new BlockBase[] { emptyBlock, emptyBlock, land("A1", LandType.Down), land("A2", LandType.Down), stationLand("S1", LandType.Down), land("A3", LandType.Down), land("A4", LandType.Down), land("A5", LandType.Down), emptyBlock, emptyBlock, emptyBlock },
                new BlockBase[] { emptyBlock, new StartPoint("start"), road("A1", RoadType.HorizontalRoad), road("A2", RoadType.HorizontalRoad), stationRoad("S1", RoadType.HorizontalRoad), road("A3", RoadType.HorizontalRoad), road("A4", RoadType.HorizontalRoad), road("A5", RoadType.TopRightIntersection), emptyBlock, emptyBlock, emptyBlock },
                new BlockBase[] { land("E4", LandType.Right), road("E4", RoadType.VerticalRoad), emptyBlock, emptyBlock, land("B1", LandType.Down), land("B2", LandType.Down), land("B3", LandType.Down), road(null, RoadType.VerticalRoad), land("B4", LandType.Down), land("B5", LandType.Down), emptyBlock },
                new BlockBase[] { stationLand("S4", LandType.Right), stationRoad("S4", RoadType.VerticalRoad), land("E2", LandType.Down), land("E1", LandType.Down), road("B1", RoadType.TopLeftIntersection), road("B2", RoadType.HorizontalRoad), road("B3", RoadType.HorizontalRoad), new ParkingLot("parkinglot"), road("B4", RoadType.HorizontalRoad), road("B5", RoadType.TopRightIntersection), emptyBlock },
                new BlockBase[] { land("E3", LandType.Right), road("E3", RoadType.BottomLeftIntersection), road("E2", RoadType.HorizontalRoad), road("E1", RoadType.HorizontalRoad), new Prison("prison"), land("C3", LandType.Down), land("S3", LandType.Down), land("C2", LandType.Down), land("C1", LandType.Down), stationRoad("S2", RoadType.VerticalRoad), land("S2", LandType.Left) },
                new BlockBase[] { emptyBlock, emptyBlock , emptyBlock , land("C4", LandType.Right) , road("C4", RoadType.BottomLeftIntersection) , road("C3", RoadType.HorizontalRoad) , stationRoad("S3", RoadType.HorizontalRoad), road("C2", RoadType.HorizontalRoad), road("C1", RoadType.HorizontalRoad) , road(null, RoadType.BottomRightIntersection), emptyBlock }
            }
        );
    }
}

public class BlockConverter : JsonConverter<BlockBase>
{

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(BlockBase);
    }

    public override BlockBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        if (!root.TryGetProperty("Type", out var typeProperty))
        {
            throw new JsonException("Missing Type property");
        }
        BlockType type = JsonSerializer.Deserialize<BlockType>(typeProperty.GetRawText(), options);
        return type switch
        {
            BlockType.None => JsonSerializer.Deserialize<EmptyBlock>(root.GetRawText(), options),
            BlockType.Road => JsonSerializer.Deserialize<Road>(root.GetRawText(), options),
            BlockType.Land => JsonSerializer.Deserialize<Land>(root.GetRawText(), options),
            BlockType.ParkingLot => JsonSerializer.Deserialize<ParkingLot>(root.GetRawText(), options),
            BlockType.Prison => JsonSerializer.Deserialize<Prison>(root.GetRawText(), options),
            BlockType.StartPoint => JsonSerializer.Deserialize<StartPoint>(root.GetRawText(), options),
            BlockType.Station => JsonSerializer.Deserialize<Station>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown block type: {type}"),
        };
    }

    public override void Write(Utf8JsonWriter writer, BlockBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}