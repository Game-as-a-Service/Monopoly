namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerTooPoorToBidEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required decimal BidPrice { get; init; }
    public required decimal HighestPrice { get; init; }
}