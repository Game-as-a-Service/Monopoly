namespace SharedLibrary.ResponseArgs.Monopoly;

public class CurrentPlayerCannotBidEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
}