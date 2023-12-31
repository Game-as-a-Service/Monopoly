namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerChooseDirectionEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string Direction { get; init; }
}