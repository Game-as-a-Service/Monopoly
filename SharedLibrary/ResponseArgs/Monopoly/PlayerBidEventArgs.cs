namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerBidEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal HighestPrice { get; init; }
}