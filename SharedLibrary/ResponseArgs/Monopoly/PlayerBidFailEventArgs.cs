namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerBidFailEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal BidPrice { get; init; }
    public required decimal HighestPrice { get; init; }
}