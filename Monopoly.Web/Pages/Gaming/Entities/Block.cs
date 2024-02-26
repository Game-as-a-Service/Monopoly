namespace Client.Pages.Gaming.Entities;

public abstract class Block
{
    public string Id { get; }
    internal Block? Up { get; set; }
    internal Block? Down { get; set; }
    internal Block? Left { get; set; }
    internal Block? Right { get; set; }

    public Block(string id)
    {
        Id = id;
    }
}

public class Land : Block
{
    protected string lot;

    public string Lot => lot;

    public Land(string id, string lot) : base(id)
    {
        this.lot = lot;
    }

}

public class StartPoint : Block
{
    public StartPoint(string id) : base(id)
    {
    }
}
public class Jail : Block
{
    public Jail(string id) : base(id)
    {
    }

}
public class ParkingLot : Block
{
    public ParkingLot(string id) : base(id)
    {
    }

}

public class Station : Land
{
    public Station(string id, string lot = "S") : base(id, lot)
    {
    }
}