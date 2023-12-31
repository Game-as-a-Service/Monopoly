namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerReadyEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string PlayerState { get; init; }
}