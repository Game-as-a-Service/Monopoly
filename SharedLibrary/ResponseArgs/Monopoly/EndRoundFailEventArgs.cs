namespace SharedLibrary.ResponseArgs.Monopoly;

public class EndRoundFailEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
}