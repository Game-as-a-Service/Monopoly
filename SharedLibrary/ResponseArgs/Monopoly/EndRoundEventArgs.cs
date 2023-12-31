namespace SharedLibrary.ResponseArgs.Monopoly;

public class EndRoundEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string NextPlayerId { get; init; }
}