using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrary.MonopolyMap;

public class MonopolyMap(string id, BlockBase[][] data)
{
    public string Id { get; } = id;
    public BlockBase[][] Data { get; } = data;

    public static MonopolyMap Parse(string data)
    {
        return JsonSerializer.Deserialize<MonopolyMap>(data, JsonSerializerOptions) ?? throw new InvalidOperationException();
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
        var type = JsonSerializer.Deserialize<BlockType>(typeProperty.GetRawText(), options);
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