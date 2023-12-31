namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerBuyBlockInsufficientFundsEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal Price { get; init; }
}