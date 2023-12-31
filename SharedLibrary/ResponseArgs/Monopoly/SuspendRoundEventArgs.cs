namespace SharedLibrary.ResponseArgs.Monopoly;

public class SuspendRoundEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required int SuspendRounds { get; init; }
}